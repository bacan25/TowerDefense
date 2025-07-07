using UnityEngine;
using UnityEditor;

/// <summary>
/// Ayudante para configurar la escena con los nuevos sistemas.
/// </summary>
public class SceneSetupHelper : MonoBehaviour
{
    [Header("Referencias Necesarias")]
    public GameObject playerPrefab;
    public GameObject gameManagerPrefab;
    public GameObject towerManagerPrefab;
    
    [Header("Materiales de Preview")]
    public Material validPreviewMaterial;
    public Material invalidPreviewMaterial;
    
    #if UNITY_EDITOR
    [ContextMenu("Configurar Escena Completa")]
    public void ConfigurarEscena()
    {
        Debug.Log("=== Iniciando configuración de escena ===");
        
        // 1. Configurar GameManager
        ConfigurarGameManager();
        
        // 2. Configurar TowerManager
        ConfigurarTowerManager();
        
        // 3. Configurar Player
        ConfigurarPlayer();
        
        // 4. Configurar Sistema de Placement
        ConfigurarPlacementSystem();
        
        // 5. Crear AudioListenerManager
        CrearAudioListenerManager();
        
        Debug.Log("=== Configuración completada ===");
    }
    
    private void ConfigurarGameManager()
    {
        GameManager gm = FindObjectOfType<GameManager>();
        if (gm == null)
        {
            Debug.LogError("No se encontró GameManager en la escena!");
            return;
        }
        
        // Agregar AudioListenerManager si no existe
        if (gm.GetComponent<AudioListenerManager>() == null)
        {
            gm.gameObject.AddComponent<AudioListenerManager>();
            Debug.Log("AudioListenerManager agregado al GameManager");
        }
    }
    
    private void ConfigurarTowerManager()
    {
        TowerManager tm = FindObjectOfType<TowerManager>();
        if (tm == null)
        {
            GameObject tmObj = new GameObject("TowerManager");
            tm = tmObj.AddComponent<TowerManager>();
            Debug.Log("TowerManager creado");
        }
        
        // Agregar TowerPreview
        if (tm.GetComponent<TowerPreview>() == null)
        {
            TowerPreview preview = tm.gameObject.AddComponent<TowerPreview>();
            
            // Asignar materiales si están disponibles
            if (validPreviewMaterial != null)
                preview.validPreviewMaterial = validPreviewMaterial;
            if (invalidPreviewMaterial != null)
                preview.invalidPreviewMaterial = invalidPreviewMaterial;
                
            Debug.Log("TowerPreview agregado al TowerManager");
        }
    }
    
    private void ConfigurarPlayer()
    {
        GameObject player = GameObject.Find("Player");
        if (player == null)
        {
            Debug.LogError("No se encontró el Player en la escena!");
            return;
        }
        
        // Buscar PlayerShooting
        PlayerShooting shooting = player.GetComponentInChildren<PlayerShooting>();
        if (shooting == null)
        {
            Debug.LogError("No se encontró PlayerShooting!");
            return;
        }
        
        // Verificar BulletPool
        if (shooting.bulletPool == null)
        {
            // Buscar BulletPool en la escena
            BulletPool pool = FindObjectOfType<BulletPool>();
            if (pool != null)
            {
                shooting.bulletPool = pool;
                Debug.Log("BulletPool asignado a PlayerShooting");
            }
            else
            {
                Debug.LogWarning("No se encontró BulletPool en la escena!");
            }
        }
        
        // Configurar SecondaryFireSystem
        SecondaryFireSystem secondaryFire = shooting.GetComponent<SecondaryFireSystem>();
        if (secondaryFire == null)
        {
            secondaryFire = shooting.gameObject.AddComponent<SecondaryFireSystem>();
            Debug.Log("SecondaryFireSystem agregado");
        }
        
        // Buscar y asignar FirePoint
        Transform firePoint = player.transform.Find("CameraHolder/FPSCamera/FirePoint");
        if (firePoint == null)
        {
            // Buscar de forma más amplia
            Transform[] allTransforms = player.GetComponentsInChildren<Transform>();
            foreach (Transform t in allTransforms)
            {
                if (t.name.Contains("FirePoint") || t.name.Contains("firePoint"))
                {
                    firePoint = t;
                    break;
                }
            }
        }
        
        if (firePoint != null)
        {
            var serializedObject = new SerializedObject(secondaryFire);
            var firePointProp = serializedObject.FindProperty("firePoint");
            if (firePointProp != null)
            {
                firePointProp.objectReferenceValue = firePoint;
                serializedObject.ApplyModifiedProperties();
                Debug.Log("FirePoint asignado a SecondaryFireSystem");
            }
        }
        else
        {
            Debug.LogWarning("No se encontró FirePoint!");
        }
    }
    
    private void ConfigurarPlacementSystem()
    {
        // Buscar o crear TowerPlacementController
        TowerPlacementController placement = FindObjectOfType<TowerPlacementController>();
        if (placement == null)
        {
            GameObject placementObj = new GameObject("TowerPlacementController");
            placement = placementObj.AddComponent<TowerPlacementController>();
            Debug.Log("TowerPlacementController creado");
        }
        
        // Configurar layer de construcción
        int constructionLayer = LayerMask.NameToLayer("ConstructionZone");
        if (constructionLayer == -1)
        {
            Debug.LogWarning("Layer 'ConstructionZone' no existe. Créalo en Edit > Project Settings > Tags and Layers");
        }
        else
        {
            placement.constructionZoneLayer = 1 << constructionLayer;
        }
    }
    
    private void CrearAudioListenerManager()
    {
        if (FindObjectOfType<AudioListenerManager>() == null)
        {
            GameManager gm = FindObjectOfType<GameManager>();
            if (gm != null)
            {
                gm.gameObject.AddComponent<AudioListenerManager>();
                Debug.Log("AudioListenerManager creado");
            }
        }
    }
    
    [ContextMenu("Crear Materiales de Preview")]
    public void CrearMaterialesPreview()
    {
        // Crear carpeta si no existe
        string folderPath = "Assets/Materials/TowerPreview";
        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            AssetDatabase.CreateFolder("Assets/Materials", "TowerPreview");
        }
        
        // Material válido (verde transparente)
        if (validPreviewMaterial == null)
        {
            Material validMat = new Material(Shader.Find("Standard"));
            validMat.name = "TowerPreviewValid";
            validMat.SetFloat("_Mode", 3); // Transparent
            validMat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            validMat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            validMat.SetInt("_ZWrite", 0);
            validMat.DisableKeyword("_ALPHATEST_ON");
            validMat.EnableKeyword("_ALPHABLEND_ON");
            validMat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            validMat.renderQueue = 3000;
            validMat.color = new Color(0, 1, 0, 0.5f);
            
            AssetDatabase.CreateAsset(validMat, folderPath + "/TowerPreviewValid.mat");
            validPreviewMaterial = validMat;
            Debug.Log("Material de preview válido creado");
        }
        
        // Material inválido (rojo transparente)
        if (invalidPreviewMaterial == null)
        {
            Material invalidMat = new Material(Shader.Find("Standard"));
            invalidMat.name = "TowerPreviewInvalid";
            invalidMat.SetFloat("_Mode", 3); // Transparent
            invalidMat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            invalidMat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            invalidMat.SetInt("_ZWrite", 0);
            invalidMat.DisableKeyword("_ALPHATEST_ON");
            invalidMat.EnableKeyword("_ALPHABLEND_ON");
            invalidMat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            invalidMat.renderQueue = 3000;
            invalidMat.color = new Color(1, 0, 0, 0.5f);
            
            AssetDatabase.CreateAsset(invalidMat, folderPath + "/TowerPreviewInvalid.mat");
            invalidPreviewMaterial = invalidMat;
            Debug.Log("Material de preview inválido creado");
        }
        
        AssetDatabase.SaveAssets();
    }
    #endif
}

#if UNITY_EDITOR
[CustomEditor(typeof(SceneSetupHelper))]
public class SceneSetupHelperEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        SceneSetupHelper helper = (SceneSetupHelper)target;
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Configuración Rápida", EditorStyles.boldLabel);
        
        if (GUILayout.Button("1. Crear Materiales de Preview", GUILayout.Height(30)))
        {
            helper.CrearMaterialesPreview();
        }
        
        if (GUILayout.Button("2. Configurar Escena Completa", GUILayout.Height(30)))
        {
            helper.ConfigurarEscena();
        }
        
        EditorGUILayout.Space();
        EditorGUILayout.HelpBox(
            "Pasos:\n" +
            "1. Primero crea los materiales de preview\n" +
            "2. Asegúrate de tener un Layer llamado 'ConstructionZone'\n" +
            "3. Ejecuta 'Configurar Escena Completa'\n" +
            "4. Revisa la consola para ver el progreso",
            MessageType.Info
        );
    }
}
#endif
