using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Gameplay
{
	public class Swiping : Swerving
	{
		[SerializeField] Renderer _renderer;
		[SerializeField] AnimationCurve progressCurve; // UnityEditor.AnimationCurveWrapperJSON:{"curve":{"serializedVersion":"2","m_Curve":[{"serializedVersion":"3","time":0.0,"value":0.0,"inSlope":0.0,"outSlope":0.0,"tangentMode":0,"weightedMode":3,"inWeight":0.0,"outWeight":0.3333333432674408},{"serializedVersion":"3","time":1.0,"value":1.0,"inSlope":218.42166137695313,"outSlope":218.42166137695313,"tangentMode":0,"weightedMode":3,"inWeight":0.0034467577934265138,"outWeight":0.0}],"m_PreInfinity":2,"m_PostInfinity":2,"m_RotationOrder":4}}
		
		Material dirtMat;
		
		public override void Play(bool b){
			base.Play(b);
			
			var materials = _renderer.materials.ToList();
			{
				if(b){
					dirtMat = Instantiate(DirtyMaterials.Instance.GetRandom());
					materials.Add(dirtMat);
				}
				
				else materials.Remove(dirtMat);
			}
			
			_renderer.materials = materials.ToArray();
		}
		
		protected override void LateUpdate(){
			base.LateUpdate();
			
			float lerp = progressCurve.Evaluate(progress);
			dirtMat.SetFloat("_transparency", Mathf.Lerp(1, 0, lerp));
		}
	}
}