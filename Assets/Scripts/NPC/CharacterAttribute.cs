using UnityEngine;


namespace MyCompany.MyGame.Data.Character
{
	[CreateAssetMenu (menuName = "Attribute/CharacterAttribute Attribute")]
	public class CharacterAttribute : ScriptableObject
	{
		public float moveSpeed;

		public float rotateSpeed;
	}

	public class EnemyAttribute : CharacterAttribute
	{
		public float touchGroundHeight;
	}
}