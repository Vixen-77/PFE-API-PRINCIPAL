README - Implémentation de l'API

Introduction

Ce projet est une API RESTful en C# utilisant ASP.NET Core et Entity Framework Core pour la gestion des patients, des professionnels de santé (proSanté) et des responsables d'hôpitaux (RespoHop). Le code est déjà commenté, et certaines parties sont marquées avec FIXME, indiquant des erreurs que je vais corriger plus tard.

Instructions pour la suite de l'implémentation

Ce qu'il reste à implémenter

rajout des methode comme par exemple :
Liste des proche....

tu trouvera tout dans le diagrame de classe et si tu veut rajouter une methode contact moi et on va discuter de sa mais pour liste patient c'est importe de la mettre les autre methode sont mise dans le diagramme de classe biensur les methode NE SONT PAS MIT DANS LES CLASSE mais dans le docier service:
par exemple je nomme PatientService.cs qui contiendra toute les methode du patient le meme principe s'applique pour Respo Hop et proSanté
je te conseil de mecrire toute les methode que chaque patient , proS et respoHop doit implementer pour qu'on en parle avec toute la team 

Si tu rencontres des difficultés, n'hésite pas à me contacter !

Points techniques

Conditions IF et SWITCH

Le code utilise des if statements pour gérer la validation des patients, des proSanté et des responsables.

Certains switch sont utilisés pour gérer des cas spécifiques comme les rôles d'utilisateurs et les états d'inscription.

Exemple :

if (patient != null && patient.isValidated == false)
{
    return "Compte en attente de validation.";
}

Gestion des ENUMS et Entity Framework

EF Core ne supporte pas les enum sous forme de string par défaut.

Pour contourner ce problème, nous utilisons des conversions ou stockons directement l'entier correspondant.

Exemple :

public enum UserRole
{
    Patient = 10,
    ProSante = 20,
    RespoHop = 30
}

Dans la base de données, ces valeurs seront enregistrées sous forme d'entiers.

Notes sur le code

Les FIXME n'éxiste plus tout les code sont ajours et optimisé et les bugs on etait enlever 

JE MEN OCCUPE PERSONELMENET des docier controlleur et le docier service NTAYA Gère selmement les methodes patient , ProS, et RespHop
les fichier QUIL NE FAUT SURTOUT PAS TOUCHé sont 

 /Services/JWTService.cs
 /Services/AuthService.cs
 /Controlleur/UserControlleur.cs
 /Controlleur/UserControlleurSignUP.cs
 le fichier Models aussi ne le trouche pas 

 SI te pense vraiment quil faut changé a de ses fichier que je vient de mentionné contactement bach je te donne l'autorisation 


NE FAIT PAS DE COMMIT SANS MON ACCORD


Le code est commenté pour faciliter la compréhension.

Architecture :

Services/ contient la logique métier.

Controllers/ expose les endpoints API.

Models/ contient les classes de données.

 