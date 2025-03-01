{
  description = "process-tracker-cli";
  inputs.nixpkgs.url = "github:NixOS/nixpkgs/nixos-unstable";

  outputs = { 
    self, 
    nixpkgs, 
    ...  
  } @ inputs: let
      system = "x86_64-linux";
      pkgs = import nixpkgs {
        inherit system;
      };
      appVersion = "1.0.201";
      dotnetVersion = "9_0";
    in {
      inherit system;

      packages."${system}" = {
        process-tracker-cli = pkgs.buildDotnetModule rec {
          pname = "process-tracker-cli";
          version = "${appVersion}";

          meta = with pkgs.lib; {
            description = "A cross-platform tool to track and report running processes";
            license = licenses.mit;
            platforms = [ system ];
          };

          dotnet-sdk = pkgs.dotnetCorePackages."sdk_${dotnetVersion}";
          dotnet-runtime = pkgs.dotnetCorePackages."runtime_${dotnetVersion}";

          src = self;

          projectFile = [
            "ProcessTracker/ProcessTracker.csproj"
          ];

          # to manually update dependencies:
          # dotnet restore --use-current-runtime --packages nuget-restore ./ProcessTrackerCLI.sln
          # nuget-to-json nuget-restore > deps.json
          # rm -r nuget-restore
          nugetDeps = ./deps.json;
          executables = [ "processtrackercli" ];
        };
      };

      defaultPackage."${system}" = self.packages."${system}".process-tracker-cli;

      nixosModules.process-tracker-cli = { config, lib, pkgs, ... }:
        let
          cfg = config.programs.process-tracker-cli;
        in {
          options.programs.process-tracker-cli = {
            enable = lib.mkEnableOption "Enable the process tracker cli";
            package = lib.mkOption {
              type = lib.types.package;
              default = self.packages.${system}.process-tracker-cli;
              description = "The package to install as the process tracker cli.";
            };
            # Extra service options (if needed)
            # serviceConfig = lib.mkOption {
            #   type = lib.types.attrs;
            #   default = {};
            #   description = "Extra configuration options for the process tracker systemd unit.";
            # };
          };

          config = lib.mkIf cfg.enable {
            home.packages = [ cfg.package ];
          };
        };
  };
}
