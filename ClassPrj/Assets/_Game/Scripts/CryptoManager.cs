using UnityEngine;
using System;
using System.Collections;
using System.Text;
using System.Security.Cryptography;

/// <summary>
/// Classe adibita alla criptazione
/// </summary>
public class CryptoManager
{
    SHA512Managed HashTool;

    /// <summary>
    /// Cripta una password con algoritmo con SHA512, con possibilita' di salarla.
    /// </summary>
    /// <param name="password">Password da criptare con SHA512</param>
    /// <param name="salaPassword">Indica se la password deve essere salata o meno</param>
    /// <returns>Restituisce una stringa che rappresenta la passweord, eventualmente salta, criptata con SHA512</returns>
    public string CriptaPassword(string password, bool salaPassword)
    {
        byte[] bytes = System.Text.Encoding.ASCII.GetBytes(password);
        System.Security.Cryptography.SHA512 sha = System.Security.Cryptography.SHA512.Create();
        byte[] hash = sha.ComputeHash(bytes);
        string pwdCriptata = Convert.ToBase64String(hash);// System.Text.Encoding.ASCII.GetString(hash);
        return pwdCriptata;

        /* POI Penseremo alla salatura
        if (salaPassword)
        {
            sale = GetSalePerPassword(password);
            pwdByte = Encoding.UTF8.GetBytes(string.Concat(pwdCriptata, sale));
            pwdCriptataByte = HashTool.ComputeHash(pwdByte);
            pwdCriptata = BitConverter.ToString(pwdCriptataByte);
            pwdCriptata = pwdCriptata.Replace("-", "");
        }
        */
    }


    /// <summary>
    /// Restituisce una stringa ("sale") da usare per salare la password indicata
    /// </summary>
    /// <param name="password">Password da salare</param>
    /// <returns>Stringa ("sale") da usare per salare la password</returns>
    private string GetSalePerPassword(string password)
    {
        //Da implementare con algoritmo a scelta
        return string.Empty;
    }

}
