using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    // Instance unique de la classe
    private static T instance;

    // Propri�t� publique pour acc�der � l'instance depuis d'autres scripts
    public static T Instance
    {
        get
        {
            // Si l'instance n'existe pas, la cr�er
            if (instance == null)
            {
                // Recherche de l'instance dans la sc�ne
                instance = FindObjectOfType<T>();

                // Si aucune instance n'est trouv�e, cr�er un nouvel objet avec le script attach�
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

    // M�thode d'initialisation (peut �tre utilis�e pour des op�rations sp�cifiques au Singleton)
    protected virtual void Awake()
    {
        // V�rifier si une instance existe d�j�
        if (instance == null)
        {
            // Si non, d�finir cette instance comme l'instance unique
            instance = this as T;
            DontDestroyOnLoad(gameObject); // Optionnel : permet de conserver l'objet Singleton lors des changements de sc�ne
        }
        else
        {
            // Si une instance existe d�j�, d�truire celle-ci
            Destroy(gameObject);
        }
    }
}
