using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    // Instance unique de la classe
    private static T instance;

    // Propriété publique pour accéder à l'instance depuis d'autres scripts
    public static T Instance
    {
        get
        {
            // Si l'instance n'existe pas, la créer
            if (instance == null)
            {
                // Recherche de l'instance dans la scène
                instance = FindObjectOfType<T>();

                // Si aucune instance n'est trouvée, créer un nouvel objet avec le script attaché
                if (instance == null)
                {
                    GameObject singletonObject = new GameObject(typeof(T).Name);
                    instance = singletonObject.AddComponent<T>();
                }
            }

            // Retourner l'instance
            return instance;
        }
    }
}
