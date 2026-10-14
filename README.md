# My Plan 💰

**My Plan** est une application de bureau moderne, légère et intuitive pour la gestion de budget et d'épargne personnelle.  
Initialement développée en WPF (.NET Core/Windows), l'application a été intégralement migrée vers **Avalonia UI 11+** et **.NET 9**, garantissant un fonctionnement 100 % natif et performant sous **Linux**, **Windows** et **macOS**.

---

## 🚀 Fonctionnalités

- **Écran d'accueil unifié** : Navigation rapide vers l'ensemble des modules de l'application.
- **Gestion des transactions** :
  - Enregistrement des dépenses et des revenus avec date, montant, description et catégorie.
  - Débit / Crédit automatique sur le solde global avec contrôle de cohérence.
  - Historique complet des transactions et possibilité de suppression avec recalcul automatique.
- **Gestion du solde global** :
  - Définition et réinitialisation du solde de départ.
  - Synchronisation en direct avec les débits/crédits effectués.
- **Gestion des catégories** :
  - Création de catégories personnalisées avec définition d'un budget mensuel estimé.
  - Édition tabulaire des catégories en temps réel via `DataGrid`.
- **Gestion de l'épargne** :
  - Définition d'objectifs d'épargne avec jauges de progression visuelles (`ProgressBar`).
  - Ajout ponctuel de fonds à une épargne avec débit du solde courant.
  - Remboursement automatique sur le solde en cas de clôture/suppression d'une épargne.
- **Analyses et graphiques interactifs (LiveChartsCore)** :
  - Visualisation des revenus et des dépenses par catégorie avec diagrammes circulaires (camemberts).
  - Analyse mensuelle avec filtrage par mois et par année.
  - Tableaux comparatifs : Montant estimé vs Montant réel vs Écart (€).

---

## 🛠️ Stack Technique

- **Framework d'application** : [.NET 9](https://dotnet.microsoft.com/) (`Microsoft.NET.Sdk`)
- **Interface graphique multiplateforme** : [Avalonia UI 11.2.5](https://avaloniaui.net/)
  - Thème : `FluentTheme` (avec support des variantes claire/sombre)
  - Police : `InterFont`
  - Composants avancés : `Avalonia.Controls.DataGrid`
- **Moteur graphique & Dataviz** : [LiveChartsCore v2](https://livecharts.dev/) (`LiveChartsCore.SkiaSharpView.Avalonia` 2.0.5)
- **Base de données & ORM** : SQLite via [Entity Framework Core 9.0.6](https://learn.microsoft.com/ef/core/)
- **Boîtes de dialogue** : `DialogHelper` interne (100 % natif Avalonia, asynchrone, multiplateforme, sans dépendance WinForms ni P/Invoke).

---

## 🗄️ Emplacement et Gestion de la Base de Données (`budget.db`)

Afin de garantir la sécurité, la pérennité et la portabilité des données de l'utilisateur, le fichier **`budget.db`** (base SQLite) n'est **jamais** stocké dans le dossier d'installation ou de release de l'application (`publish/`).

Il est stocké dans le répertoire système standard réservé aux données d'applications locales de l'utilisateur (**`LocalApplicationData`**).

### Emplacements par système d'exploitation

| Système d'exploitation | Emplacement absolu de `budget.db` |
| :--- | :--- |
| **Linux** | `~/.local/share/MyPlan/budget.db`<br>*(ex. `/home/<utilisateur>/.local/share/MyPlan/budget.db`)* |
| **Windows** | `%LOCALAPPDATA%\MyPlan\budget.db`<br>*(ex. `C:\Users\<Utilisateur>\AppData\Local\MyPlan\budget.db`)* |
| **macOS** | `~/Library/Application Support/MyPlan/budget.db` |

### Pourquoi ce choix d'architecture ?

1. **Préservation des données lors des mises à jour** :  
   Si la base de données était stockée dans le répertoire de l'exécutable (`publish/`), le remplacement ou la mise à jour de l'application détruirait l'historique et le solde de l'utilisateur. En la plaçant dans `LocalApplicationData`, l'application peut être re-compilée, mise à jour ou déplacée sans aucun impact sur les comptes existants.
2. **Permissions et sécurité** :  
   Les systèmes d'exploitation modernes protègent les répertoires d'installation (tels que `/usr/bin/`, `/opt/` ou `C:\Program Files\`), interdisant l'écriture directe de fichiers. Le dossier `LocalApplicationData` dispose toujours des droits d'écriture pour l'utilisateur courant.
3. **Migration automatique au premier lancement** :  
   Si un fichier `budget.db` préexistant est présent aux côtés du binaire (par exemple lors d'un déploiement initial), la méthode `BudgetContext.GetDatabasePath()` le copie automatiquement vers le répertoire utilisateur lors du premier démarrage.

---

## 💻 Compilation et Exécution

### Prérequis
- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)

### Lancer l'application en mode développement

Depuis le terminal :
```bash
dotnet run --project MyPlan/MyPlan/MyPlan.csproj
```

*(Si `dotnet` est installé dans un répertoire local comme `~/.dotnet`, remplacez `dotnet` par `~/.dotnet/dotnet` ou ajoutez-le à votre variable d'environnement `PATH`).*

---

## 📦 Génération des Binaires Autonomes (Publication)

L'application peut être publiée en exécutable unique autonome (*self-contained single-file*), ne nécessitant aucune installation préalable de .NET sur la machine cible :

### 🐧 Pour Linux (64-bit)
```bash
dotnet publish MyPlan/MyPlan/MyPlan.csproj -c Release -r linux-x64 --self-contained true -p:PublishSingleFile=true
```
- **Sortie** : `MyPlan/MyPlan/bin/Release/net9.0/linux-x64/publish/MyPlan`
- **Lancement** : `./MyPlan/MyPlan/bin/Release/net9.0/linux-x64/publish/MyPlan`

### 🪟 Pour Windows (64-bit)
```bash
dotnet publish MyPlan/MyPlan/MyPlan.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```
- **Sortie** : `MyPlan/MyPlan/bin/Release/net9.0/win-x64/publish/MyPlan.exe`
- **Lancement** : Double-clic sur `MyPlan.exe`

### 🍎 Pour macOS (Intel & Apple Silicon)
```bash
# Pour Apple Silicon (M1/M2/M3/M4)
dotnet publish MyPlan/MyPlan/MyPlan.csproj -c Release -r osx-arm64 --self-contained true -p:PublishSingleFile=true

# Pour processeurs Intel
dotnet publish MyPlan/MyPlan/MyPlan.csproj -c Release -r osx-x64 --self-contained true -p:PublishSingleFile=true
```

---

## 🐧 Intégration sur le Bureau Linux (Tiroir d'applications)

Pour intégrer **My Plan** directement dans le lanceur d'applications sous Linux (GNOME, KDE, XFCE, etc.) :

1. Placer l'icône dans le dossier d'icônes local :
   ```bash
   mkdir -p ~/.local/share/icons/hicolor/256x256/apps
   cp MyPlan/MyPlan/icon.png ~/.local/share/icons/hicolor/256x256/apps/myplan.png
   ```

2. Créer le fichier `~/.local/share/applications/myplan.desktop` :
   ```ini
   [Desktop Entry]
   Version=1.0
   Type=Application
   Name=My Plan
   GenericName=Gestion de Budget
   Comment=Application de gestion de budget et épargne personnelle
   Exec=/chemin/absolu/vers/publish/MyPlan
   Path=/chemin/absolu/vers/publish
   Icon=myplan
   Terminal=false
   Categories=Office;Finance;
   StartupWMClass=MyPlan
   ```

3. Mettre à jour la base des raccourcis :
   ```bash
   update-desktop-database ~/.local/share/applications/
   ```

---

## 📂 Structure du Projet

```text
My-Plan/
├── README.md                      # Documentation du projet
├── .gitignore                     # Règles d'exclusion Git pour .NET
└── MyPlan/
    ├── MyPlan.sln                 # Solution .NET
    └── MyPlan/
        ├── MyPlan.csproj          # Configuration du projet et packages Avalonia 11+
        ├── Program.cs             # Point d'entrée de l'application (BuildAvaloniaApp)
        ├── App.axaml (.cs)        # Cycle de vie, thèmes et styles globaux
        ├── DialogHelper.cs        # Boîtes de dialogue multiplateformes natives
        │
        ├── models/                # Couche de données et logique métier
        │   ├── BudgetContext.cs   # Contexte EF Core SQLite et gestion du chemin de stockage
        │   ├── AppState.cs        # État global et gestion des débits/crédits
        │   ├── Transaction.cs     # Modèle de transaction
        │   ├── Categorie.cs       # Modèle de catégorie budgétaire
        │   ├── Epargne.cs         # Modèle d'épargne avec calcul de progression
        │   └── GlobalBalance.cs   # Modèle de solde global
        │
        └── Vues (Fenêtres Avalonia) :
            ├── Accueil.axaml (.cs)                # Menu principal de l'application
            ├── MainWindow.axaml (.cs)             # Gestion et historique des transactions
            ├── BalanceWindow.axaml (.cs)          # Gestion du solde bancaire
            ├── CategorieEditView.axaml (.cs)      # Édition tabulaire des catégories
            ├── EpargneView.axaml (.cs)            # Gestion des épargnes et objectifs
            ├── RevenueView.axaml (.cs)            # Liste et camembert des revenus
            ├── DepenseView.axaml (.cs)            # Liste et camembert des dépenses
            ├── RevenuMensuelView.axaml (.cs)      # Analyse mensuelle des revenus
            └── DepensesMensuellesView.axaml (.cs) # Analyse mensuelle des dépenses
```

---

## 📄 Licence

Ce projet est distribué sous licence propriétaire / personnelle.
