using UnityEngine ;

   [System.Serializable]
   public class WheelSlice {
      public Sprite Icon;

      [Tooltip("Sprite resolution")]
      public Vector2 resolution;

      [Tooltip("Item name:")]
      public string name;

      [Tooltip ("Probability in %")] 
      [Range (0f, 100f)] 
      public float Chance = 100f;

      [Tooltip("Rarity represented by color")]
      public Color rarity; 

      [HideInInspector] public int Index ;
      [HideInInspector] public double _weight = 0f ;
   }
