using UnityEngine;
using System.Collections.Generic;

public class gen : MonoBehaviour
{
    public GameObject cubePrefab;
    public int iterations = 4;
    public float angle = 25f;
    public float length = 1f;
    public int roundingDecimals = 3; // controla la precisión para detectar posiciones iguales

    private string axiom = "F";
    private string currentString;
    private Dictionary<char, string> rules = new Dictionary<char, string>();
    private HashSet<string> occupiedPositions = new HashSet<string>();

    void Start()
    {
        rules.Add('F', "F[+F]F[-F]F[\\F][&F][^F]");

        currentString = axiom;
        for (int i = 0; i < iterations; i++)
        {
            currentString = ApplyRules(currentString);
        }

        GenerateTree(currentString);
    }

    string ApplyRules(string input)
    {
        string result = "";
        foreach (char c in input)
        {
            if (rules.ContainsKey(c))
                result += rules[c];
            else
                result += c.ToString();
        }
        return result;
    }

    void GenerateTree(string instructions)
    {
        occupiedPositions.Clear();
        Stack<TransformInfo> transformStack = new Stack<TransformInfo>();
        Vector3 position = Vector3.zero;             // punto "base" desde donde crece la siguiente sección
        Quaternion rotation = Quaternion.identity;   // orientación "tortuga"

        // Padre para mantener jerarquía ordenada
        Transform parent = new GameObject("LSystemTree").transform;
        parent.SetParent(this.transform, false);

        foreach (char c in instructions)
        {
            if (c == 'F')
            {
                // center = posición del centro del cubo (si asumimos pivot en el centro)
                Vector3 center = position + rotation * Vector3.up * (length * 0.5f);

                // clave redondeada para comparar posiciones (evita problemas por floats)
                string key = PosKey(center);

                // Si ya hay un cubo en esa posición (aprox), no lo instanciamos de nuevo
                if (!occupiedPositions.Contains(key))
                {
                    GameObject cube = Instantiate(cubePrefab, center, rotation, parent);
                    cube.transform.localScale = new Vector3(0.2f, length, 0.2f);
                    occupiedPositions.Add(key);
                }

                // avanzamos la "tortuga" la longitud completa (independiente de si instanciamos)
                position += rotation * Vector3.up * length;
            }
            else if (c == '+') rotation *= Quaternion.Euler(0, 0, angle);
            else if (c == '-') rotation *= Quaternion.Euler(0, 0, -angle);
            else if (c == '&') rotation *= Quaternion.Euler(angle, 0, 0);
            else if (c == '^') rotation *= Quaternion.Euler(-angle, 0, 0);
            else if (c == '/') rotation *= Quaternion.Euler(0, angle, 0);
            else if (c == '\\') rotation *= Quaternion.Euler(0, -angle, 0);
            else if (c == '[')
            {
                transformStack.Push(new TransformInfo { position = position, rotation = rotation });
            }
            else if (c == ']')
            {
                TransformInfo ti = transformStack.Pop();
                position = ti.position;
                rotation = ti.rotation;
            }
        }
    }

    string PosKey(Vector3 v)
    {
        // Redondeamos a N decimales para manejar pequeñas diferencias de float
        return $"{v.x.ToString($"F{roundingDecimals}")}|{v.y.ToString($"F{roundingDecimals}")}|{v.z.ToString($"F{roundingDecimals}")}";
    }

    struct TransformInfo
    {
        public Vector3 position;
        public Quaternion rotation;
    }
}
