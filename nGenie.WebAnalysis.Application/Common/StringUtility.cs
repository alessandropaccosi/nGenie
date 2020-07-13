using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace nGenie.WebAnalysis.Application.Common
{
    public class StringUtility
    {
        public StringUtility()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public static bool IsNullOrEmpty(string s)
        {
            return (s == null || s == "");
        }

        public static bool IsInteger(string s)
        {
            try
            {
                int.Parse(s);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsLong(string s)
        {
            try
            {
                long.Parse(s);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static string RemoveEnd(string source, string match)
        {
            if (source.ToUpper().EndsWith(match.ToUpper()))
                return source.Substring(0, source.Length - match.Length);
            else
                return source;
        }

        public static string EncodeToBase64(string toEncode)
        {
            byte[] toEncodeAsBytes = System.Text.ASCIIEncoding.ASCII.GetBytes(toEncode);
            return System.Convert.ToBase64String(toEncodeAsBytes);
        }

        public static string DecodeFromBase64(string encodedData)
        {
            byte[] encodedDataAsBytes = System.Convert.FromBase64String(encodedData);
            return System.Text.ASCIIEncoding.ASCII.GetString(encodedDataAsBytes);
        }

        public static bool EndWithoutCase(string source, string value)
        {
            bool result = false;
            if (source != null && value != null)
                result = source.ToLower().EndsWith(value.ToLower());

            return result;
        }

        /// <summary>
        /// Permette di stabilire se un array contiene l'elemento specificato.
        /// Il controllo viene effettuato senza case.
        /// </summary>
        /// <param name="source">Array sorgente</param>
        /// <param name="value">Valore da cercare</param>
        /// <returns></returns>
        public static bool Contains(string[] source, string value)
        {
            bool result = false;
            if (source != null && value != null)
            {
                foreach (string s in source)
                {
                    if (s != null
                        && s.ToLower() == value.ToLower())
                    {
                        result = true;
                        break;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Restituisce la stringa specificata se il suo valore non è null, altrimenti la stringa vuota.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string GetValueNotNull(string s)
        {
            return s == null ? "" : s;
        }

        public static string ToString(string separatore, IEnumerable<string> listaStringhe)
        {
            return ToString(separatore, listaStringhe, false);
        }

        /// <summary>
        /// Permette di convertire in stringa una lista di stringhe utilizzando il separatore specificato.
        /// </summary>
        /// <param name="listaStringhe">La lista di stringhe</param>
        /// <param name="separatore">Il separatore da utilizzare</param>
        /// <param name="removeSeparatoreFromValues">True se il separatore deve essere cancellato da ciascun elemento della lista</param>
        /// <returns></returns>
        public static string ToString(string separatore, IEnumerable<string> listaStringhe,
            bool removeSeparatoreFromValues)
        {
            StringBuilder result = new StringBuilder();
            bool isFirstString = true;
            foreach (string item in listaStringhe)
            {
                string itemValue = removeSeparatoreFromValues ?
                    Replace(separatore, "", item)
                    : item;

                if (isFirstString)
                {
                    result.Append(itemValue);
                    isFirstString = false;
                }
                else
                    result.Append(string.Format("{0}{1}", separatore, itemValue));
            }

            return result.ToString();
        }

        /// <summary>
        /// Permette di convertire in stringa una lista di stringhe utilizzando una stringa separatore.
        /// </summary>
        public static string ToString(string separatore, params string[] listaStringhe)
        {
            List<string> result = new List<string>();
            foreach (string s in listaStringhe)
            {
                if (!string.IsNullOrEmpty(s))
                    result.Add(s);
            }

            return ToString(separatore, result);
        }

        /// <summary>
        /// Permette di effettuare sostituzioni ai caratteri di una stringa.
        /// Supporta anche stringhe nulle.
        /// </summary>
        /// <param name="oldChar">Vecchio carattere da sostituire</param>
        /// <param name="newChar">Valore che deve assumere il vecchio carattere</param>
        /// <param name="sourceString">Stringa su cui effettuare le sostituzioni</param>
        /// <returns></returns>
        public static string Replace(string oldValue, string newValue, string sourceString)
        {
            string result = sourceString;
            if (sourceString != null)
                result = sourceString.Replace(oldValue, newValue);

            return result;
        }



        /// <summary>
        /// Permette di ottenere la sottostringa lunga al massimo il numero dei caratteri specificato.
        /// È possibile specificare qualsiasi numero di caratteri, non sarà sollevata eccezione.
        /// </summary>
        /// <param name="numeroMassimoCaratteri"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetSubstring(int numeroCaratteri, string value)
        {
            string result = "";
            if (!string.IsNullOrEmpty(value)
                && numeroCaratteri > 0)
            {
                result = value.Substring(0, Math.Min(numeroCaratteri, value.Length));
            }

            return result;
        }

        /// <summary>
        /// Restituisce una stringa con la lunghezza specificata riempita di spazi a destra.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="lenght">Lunghezza della stringa da restituire</param>
        /// <returns></returns>
        public static string PadRightWithoutOverflow(string value, int lenght)
        {
            string result = "".PadRight(lenght);
            if (!string.IsNullOrEmpty(value))
            {
                if (value.Length >= lenght)
                {
                    result = value.Substring(0, lenght);
                }
                else
                    result = value.PadRight(lenght);
            }

            return result;
        }

        /// <summary>
        /// Restituisce la lunghezza della stringa specificata.
        /// </summary>
        /// <param name="codiceFiscale"></param>
        /// <returns></returns>
        public static int GetLenght(string codiceFiscale)
        {
            int result = 0;
            if (!string.IsNullOrEmpty(codiceFiscale))
                result = codiceFiscale.Length;

            return result;
        }

        /// <summary>
        /// Prende come parametro una stringa in formato domain\username
        /// e restituisce il dominio. Non solleva eccezioni.
        /// </summary>
        public static string GetDomain(string dominioUsername)
        {
            string result = "";
            if (!string.IsNullOrEmpty(dominioUsername))
            {
                string[] splitString = dominioUsername.Split('\\');
                if (splitString.Length > 1)
                    result = splitString[0];
            }

            return result;
        }

        /// <summary>
        /// Prende come parametro una stringa in formato dominio\username
        /// e restituisce la username. Non solleva eccezioni.
        /// </summary>
        public static string GetUsernameWithoutDomain(string dominioUsername)
        {
            string result = "";
            if (!string.IsNullOrEmpty(dominioUsername))
            {
                string[] splitString = dominioUsername.Split('\\');
                switch (splitString.Length)
                {
                    case 0:
                    case 1:
                        result = dominioUsername;
                        break;

                    default:
                        result = splitString[1];
                        break;
                }
            }

            return result;
        }
    }
}