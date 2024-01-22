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

    // Méthode d'initialisation (peut être utilisée pour des opérations spécifiques au Singleton)
    protected virtual void Awake()
    {
        // Vérifier si une instance existe déjà
        if (instance == null)
        {
            // Si non, définir cette instance comme l'instance unique
            instance = this as T;
            DontDestroyOnLoad(gameObject); // Optionnel : permet de conserver l'objet Singleton lors des changements de scène
        }
        else
        {
            // Si une instance existe déjà, détruire celle-ci
            Destroy(gameObject);
        }
    }
}
