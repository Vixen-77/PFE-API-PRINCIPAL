namespace APIAPP.Enums
{
    public enum UserState
    {
        Pieton =0,      // Correction : PascalCase
        Passager =1,   
        Conducteur =2  // Correction : "passagé" -> "Passager" (orthographe)
    }
} // en effet il faut rajouter un nombre car test entity ne supporte pas les enum a string et les convertie direct en numero dou le changement
