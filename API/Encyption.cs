using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

public class Encryption : MonoBehaviour
{
    //65535
    //String.fromCharCode(65535).charCodeAt(0)

    static int MaxChar = 55000;
    //Buffer has conversion error around 55000-60000 alhough 60000+ is good

    public static void GenKeysSync(out string Pri, out string Pub, double keyLength = 100)
    {
        string key = "";
        for (int i = 0; i < keyLength; i++)
        {
            key += (Char)UnityEngine.Random.Range(0, MaxChar);
        }

        byte[] bytes = Encoding.Default.GetBytes(key);
        key = Encoding.UTF8.GetString(bytes);

        Pri = key;
        Pub = key;
    }

    public static string Encrypt(string publicKey, string Text)
    {
        string Ret = "";
        for (int i = 0; i < Text.Length; i++)
        {
            int shift = i % publicKey.Length;
            int N = (int)Text[i] + (int)publicKey[shift];
            N = N % MaxChar;
            if (N < 0)
                N += MaxChar;
            Ret += (Char)(N);
        }
        return Ret;
    }

    public static string Decrypt(string privateKey, string Text)
    {
        string Ret = "";
        for (int i = 0; i < Text.Length; i++)
        {
            int shift = i % privateKey.Length;
            int N = (int)Text[i] - (int)privateKey[shift];
            N = N % MaxChar;
            if (N < 0)
                N += MaxChar;
            Ret += (Char)(N);
        }
        return Ret;
    }
}