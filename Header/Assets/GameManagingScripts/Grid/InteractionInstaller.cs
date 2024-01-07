using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionInstaller : MonoBehaviour
{
    [SerializeField] InteractionDefines.InteractionInstallerProps[] interactionInstaller;
    // Start is called before the first frame update
    void Awake()
    {
        for (int i = 0; i < interactionInstaller.Length; i++)
        {
            Managers.instance.Grid.AddInteraction(interactionInstaller[i]);

        }
    }
    private void OnDrawGizmos()
    {
        for (int i = 0; i < interactionInstaller.Length; i++)
        {
            Gizmos.DrawCube(new Vector3(interactionInstaller[i].interactionPosition.x, interactionInstaller[i].interactionPosition.y, 0), Vector3.one);

        }
    }
}
