# StoreManagement

Application Windows Forms de gestion de magasin en C#/.NET 8 et SQLite.

## État

Le dépôt contient une base applicative avec navigation WinForms, gestion initiale des produits et catégories, mouvements de stock SQLite, ainsi que des écrans d’accueil, ventes et connexion. Les écrans Ventes et Connexion restent des maquettes et ne doivent pas être considérés comme des fonctions de caisse ou d’authentification sécurisées.

## Prérequis

- Windows 10/11
- .NET 8 SDK
- Visual Studio 2022 avec le workload « Développement Desktop .NET »

## Lancer

Ouvrir `StoreManagement.sln` dans Visual Studio, définir `StoreManagement.Presentation` comme projet de démarrage, restaurer les packages puis lancer avec F5.

La base `store.db` est créée dans le dossier de sortie de l’application au premier affichage des pages Produits, Catégories ou Stock. Sauvegarder ce fichier pour conserver les données.

> Le dépôt n’inclut pas encore de build Windows CI ni de publication d’installateur. Vérifier la compilation et l’exécution sur Windows avant usage réel.
