# bravectl
A lightweight C# command-line tool that interacts with the Brave API.

## ✨ Features
- Search the web via Brave's API
- Open URLs, manage tabs (if applicable via the Brave API)
- Extendable and modular — built with .NET CLI tooling

## 🚀 Installation
Make sure you have [.NET 6+ SDK](https://dotnet.microsoft.com/download) installed.

  ```bash
  # Clone the repository
  git clone https://github.com/yourusername/bravely-cli.git
  cd bravely-cli
  
  # Build the project
  dotnet build
  
  # (Optional) Run directly
  dotnet run -- search "hello brave world"
  
  # (Optional) Install as a global tool (if structured as a .NET Tool)
  dotnet tool install --global --add-source ./nupkg Bravely

🛠️ Usage
bravely search "c# cli project templates"

# Open a specific URL (if supported)
bravely open https://github.com

🙌 Contributing
Contributions are welcome! Feel free to open issues, suggest features, or submit pull requests.

Made with ❤️ for terminal users and privacy nerds.

