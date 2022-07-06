using UnityEngine;

public class NPCManager : SceneObjectSingleton<NPCManager>
{
	public NPC[] nonPlayableCharacters;
	public Collider[] walkables;
	
	public void Reiterate(bool toggleUIValue){
		if(toggleUIValue)
			foreach(var npc in nonPlayableCharacters)
				StartCoroutine(npc.Start());
	}
}