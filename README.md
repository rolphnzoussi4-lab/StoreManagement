# StoreManagement

Application Windows Forms de gestion de magasin construite en C#/.NET 8 avec SQLite.

## Fonctionnalités principales

- Dashboard d’accueil
- Gestion des produits
- Gestion des catégories
- Gestion du stock avec mouvements de stock
- Écran de ventes
- Écran de connexion
- Base SQLite pour le stockage local

## Prérequis

- Windows 10/11
- .NET 8 SDK
- Visual Studio 2022 avec le workload « Développement Desktop .NET »

## Démarrage

1. Ouvrir la solution `StoreManagement.sln`
2. Définir `StoreManagement.Presentation` comme projet de démarrage
3. Restaurer les packages NuGet
4. Lancer avec F5

## Données

La base SQLite est générée dans le dossier de sortie de l’application (`store.db`) lors des premiers accès aux écrans produits, catégories et stock.

## Limite actuelle

Cette application s’inscrit dans une base de gestion de magasin en cours de maturation. Les écrans de caisse et d’authentification restent des modules de démonstration/maquettes, et l’application doit être encore stabilisée et testée en environnement Windows avant un usage réel en production.
