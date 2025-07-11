using UnityEngine;

/// <summary>
/// Script alternativo para crear materiales de preview manualmente.
/// Adjunta este script a cualquier GameObject y asigna los materiales creados.
/// </summary>
public class CreatePreviewMaterials : MonoBehaviour
{
    [Header("Materiales Creados Manualmente")]
    [Tooltip("Crear un material nuevo, configurarlo como transparente y asignarlo aquí")]
    public Material validPreviewMaterial;
    public Material invalidPreviewMaterial;
    
    [Header("Instrucciones")]
    [TextArea(10, 20)]
    public string instrucciones = @"CÓMO CREAR LOS MATERIALES MANUALMENTE:

1. MATERIAL VÁLIDO (Verde):
   - Click derecho en Project > Create > Material
   - Nombrarlo 'TowerPreviewValid'
   - En Inspector:
     * Shader: Universal Render Pipeline/Lit (o Standard)
     * Surface Type: Transparent
     * Base Map color: Verde con Alpha 0.5 (128)
     * Guardar

2. MATERIAL INVÁLIDO (Rojo):
   - Click derecho en Project > Create > Material
   - Nombrarlo 'TowerPreviewInvalid'
   - En Inspector:
     * Shader: Universal Render Pipeline/Lit (o Standard)
     * Surface Type: Transparent
     * Base Map color: Rojo con Alpha 0.5 (128)
     * Guardar

3. ASIGNAR:
   - Arrastra los materiales creados a los campos de arriba
   - Busca TowerManager en la escena
   - En el componente TowerPreview, asigna estos materiales";
   
    void Start()
    {
        // Buscar y asignar automáticamente si es posible
        if (validPreviewMaterial != null || invalidPreviewMaterial != null)
        {
            TowerPreview preview = FindObjectOfType<TowerPreview>();
            if (preview != null)
            {
                if (validPreviewMaterial != null && preview.validPreviewMaterial == null)
                {
                    preview.validPreviewMaterial = validPreviewMaterial;
                    Debug.Log("Material válido asignado a TowerPreview");
                }
                
                if (invalidPreviewMaterial != null && preview.invalidPreviewMaterial == null)
                {
                    preview.invalidPreviewMaterial = invalidPreviewMaterial;
                    Debug.Log("Material inválido asignado a TowerPreview");
                }
            }
        }
    }
    
    [ContextMenu("Buscar Materiales Existentes")]
    void BuscarMaterialesExistentes()
    {
        // Buscar en Resources
        if (validPreviewMaterial == null)
        {
            validPreviewMaterial = Resources.Load<Material>("TowerPreviewValid");
        }
        
        if (invalidPreviewMaterial == null)
        {
            invalidPreviewMaterial = Resources.Load<Material>("TowerPreviewInvalid");
        }
        
        // Mensaje de resultado
        if (validPreviewMaterial != null || invalidPreviewMaterial != null)
        {
            Debug.Log("Materiales encontrados!");
        }
        else
        {
            Debug.Log("No se encontraron materiales. Créalos manualmente siguiendo las instrucciones.");
        }
    }
}
