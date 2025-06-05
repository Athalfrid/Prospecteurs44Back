# 🚀 Prospecteurs44Back

Backend API en .NET pour l’application Prospecteurs44, destinée à gérer les échanges, alertes et interactions au sein de l’association de prospecteurs.

---

## 🛠️ Fonctionnalités principales

- API RESTful sécurisée pour gérer les membres, alertes, publications, etc.
- Authentification et gestion des rôles (membres, modérateurs, admins)
- Gestion des alertes d’objets perdus / retrouvés
- Système de modération pour assurer un espace respectueux
- Connexion avec base de données MongoDB

---

## 📦 Prérequis

- [.NET 6 SDK (ou version compatible)](https://dotnet.microsoft.com/download)
- MongoDB (local ou hébergé)
- Visual Studio / VS Code (avec extension C#)

---

## 🚀 Installation et lancement

```bash
# Cloner le dépôt
git clone https://github.com/Athalfrid/Prospecteurs44Back.git
cd Prospecteurs44Back

# Restaurer les dépendances
dotnet restore

# Configurer la connexion MongoDB dans appsettings.json (exemple à adapter)
# "ConnectionStrings": {
#   "MongoDb": "mongodb://localhost:27017/prospecteurs44"
# }

# Lancer l’API
dotnet run
