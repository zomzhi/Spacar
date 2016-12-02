using UnityEngine;


namespace MyCompany.MyGame.Data.Character
{
	[CreateAssetMenu (menuName = "Attribute/CharacterAttribute")]
	public class CharacterAttribute : ScriptableObject
	{
		public float moveSpeed;

		public float rotateSpeed;
	}
}