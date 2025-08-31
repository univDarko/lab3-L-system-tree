using UnityEngine;
using System.Collections.Generic;

public class gen : MonoBehaviour
{
    public GameObject cubePrefab;
    public int iterations = 4;
    public float angle = 25f;
    public float length = 1f;

    private string axiom = "F";
    private string currentString;
    private Dictionary<char, string> rules = new Dictionary<char, string>();

    void Start()
    {
        // Reglas: ahora generan ramas en distintas direcciones (3D)
        rules.Add('F', "F[+F]F[-F]F[/F][\\F][&F][^F]");

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
        Stack<TransformInfo> transformStack = new Stack<TransformInfo>();
        Vector3 position = Vector3.zero;
        Quaternion rotation = Quaternion.identity;

        foreach (char c in instructions)
        {
            if (c == 'F')
            {
                // Instanciar cubo
                GameObject cube = Instantiate(cubePrefab, position, rotation);
                cube.transform.localScale = new Vector3(0.2f, length, 0.2f);

                // Avanzar hacia arriba (local Y del cubo)
                position += rotation * Vector3.up * length;
            }
            else if (c == '+')
            {
                rotation *= Quaternion.Euler(0, 0, angle); // rotar en Z
            }
            else if (c == '-')
            {
                rotation *= Quaternion.Euler(0, 0, -angle);
            }
            else if (c == '&')
            {
                rotation *= Quaternion.Euler(angle, 0, 0); // rotar en X
            }
            else if (c == '^')
            {
                rotation *= Quaternion.Euler(-angle, 0, 0);
            }
            else if (c == '/')
            {
                rotation *= Quaternion.Euler(0, angle, 0); // rotar en Y
            }
            else if (c == '\\')
            {
                rotation *= Quaternion.Euler(0, -angle, 0);
            }
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

    struct TransformInfo
    {
        public Vector3 position;
        public Quaternion rotation;
    }
}
