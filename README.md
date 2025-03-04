UPDATE Actuelle:
rajout les classe enumération suivante:
AccountStatus.cs  
Device.cs
Speciality.cs
UserState.cs 

rajout des classe Models (a migrer plus tard a la base apres laccord de la prof )
Centre.cs
Patient.cs
Proche.cs
ProS.cs
ResHop.cs
USER.cs
pour "TestEntity" il sera garder uniquement pour etre sur aue API communique et lance les migration sans oncombre
un docier interface a était rajouter au future il contiadra des future interface et contrat a signé avec dautre classe
le docier Migration est automatisé tout les fichier de Migration se génere de manière auto par API au lancement des migartion avec la commende :
dotnet ef migrations add Add-nomde table a rajouter-
dotnet update databe 
je rapple qu'on utilise SSMS Microsoft SQL Serveur Express noublier ne changé le nom de votre serveur correspondant au votre dans appsettings.json 
le docier controlleur et docier services ne sont pas encore opéracionel leur code est en dévlopement dans un fichier séparé et sera copier ultérieurement pour comprendre le role de chaquun :
le controlleur --> gère les requette HTTP venant du front-end (REACT) 
                                                 de Lautre API
                                                 
le Service -->le centre et le coeur du focntionement de cette API c'est la principale elle géra principalement les Login, Signin,Logout,Signout les connextion acces au info principalement utilisateur, bot de sécurté automatisation et calcule en temps reel et gere les notif , les code des service ne s'exuce pas de manière aléatoir mais leur code est exécuté que l'orsque le controlleur en a besoin :

--requete extèrieur-->API-->controlleur-->"déclanchmenet dapple de service X"--->Controlleur apple le service conserné--->éxecute le code du service---->finis --->renvois une reponse 
