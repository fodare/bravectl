# bravectl

[![build_publish_nugget](https://github.com/fodare/bravectl/actions/workflows/CI-CD.yml/badge.svg)](https://github.com/fodare/bravectl/actions/workflows/CI-CD.yml)

A .NET tool that performs web searches using the Brave API. It functions by accepting command-line arguments and options, constructing a Brave API query parameter model, making an HTTP request to the Brave API, and displaying the search results in the console using spectre-console.

| ![demo.png](https://github.com/fodare/media/blob/main/bravectl/demo.png?raw=true) | ![help message](https://github.com/fodare/media/blob/main/bravectl/helpMessage.png?raw=true)|
| ------- | --- |

## 🔧 Capabilities

- 🌐 Search the web using Brave – Perform web searches with Brave Search and enjoy fewer ads and more privacy-focused results.

- 📄 Extract and read web content – Convert a web page into plain text and display it directly in the console for easy reading.

## 🚀 Installation

- Make sure you have [.NET 6+ SDK](https://dotnet.microsoft.com/download) installed.
- You have a valid [Brave API Key](https://brave.com/search/api/).

You can build and use the tool locally or download executable version from [nuget.org](https://www.nuget.org/).

### Build and use locally

  ```bash
  # Clone the repository
  git clone https://github.com/fodare/bravectl.git
  cd bravectl

  # Restore dependencies
  dotnet restore
  
  # Build the project
  dotnet build
  
  # (Optional) Run test
  dotnet run test

  # Export brave api key. See https://brave.com/search/api/.
  export braveAPIKey=Enter-API-Key

  # (Optional) Run directly using alias from project dir.
  dotnet run -- search -f web -q "Where is the ISS"
  # dotnet run -- search --filter web --query "Where is the ISS"

  # Package tool locally. To overide tool name / command, see ToolCommandName in ...csproj file.
  # From project parent directory, run
  dotnet pack
  
  # (Optional) Install as a global tool (if structured as a .NET Tool)
  dotnet tool install --global --add-source ./nupkg bravectl

  # Run application from any dir.
  bravectl search -f web -q "Where is the ISS"

  # Convert webpage to text and print to console.
  bravectl extract "https://www.astroviewer.net/iss/en/"
  ```

### Download from nuget.org

```bash
# See https://www.nuget.org/packages/bravectl
dotnet tool install -g bravectl
```

## 🙌 Contributing

Contributions are welcome! Please feel free to open issues, suggest features, or submit pull requests.

Made with ❤️ for terminal users and privacy nerds.
