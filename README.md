# UDPMonitor

UDPMonitor is a WPF desktop application for monitoring and displaying incoming and outgoing UDP messages. The solution is organized into multiple projects to separate core utilities, business services and the UI. Automated tests are included.

## Projects overview
- `UDPMonitor` - WPF application (user interface)
- `UDPMonitor.Business` - services that manage UDP channels (inbound/outbound)
- `UDPMonitor.Core` - shared utilities, configuration and export helpers
- `UDPMonitor.Tests` - automated tests (unit tests)

## Requirements
- .NET 9 SDK
- Visual Studio 2022/2023 or a compatible editor (e.g. VS Code + C# extension)

## Build, test and run
1. Restore and build the solution:

   `dotnet build`

2. Run the tests:

   `dotnet test`

3. Run the WPF app (from the WPF project):

   `dotnet run --project UDPMonitor/UDPMonitor.csproj`

## Configuration
Configuration is handled by `UDPMonitor.Core` and `UDPMonitor.Business`. See `UDPMonitor.Business/Configuration` and the classes under `UDPMonitor.Core/Configuration` for available settings (for example the UDP port used for inbound/outbound channels).

## Contributing
- Open an issue to report bugs or propose features.
- Submit a pull request with a clear description of changes.
- Run `dotnet test` before opening a PR and keep changes small and covered by tests when possible.

## License
No license file is included in the repository. Add a `LICENSE` file (e.g. MIT) if you plan to make the project open source.

## Contact
For questions about the project structure or behavior, review the source code in the repository or open an issue.
