# Game Launcher

This project is a WPF application designed to serve as a game launcher. It allows users to configure various settings for their gaming experience, including difficulty presets, screen resolution, and key bindings for game controls.

## Features

- **Difficulty Selection**: Users can choose from various difficulty presets to tailor their gaming experience.
- **Screen Configuration**: Options to set the application to fullscreen or windowed mode, along with adjustable screen width and height.
- **Key Bindings**: Users can customize key bindings for movement, confirmation, pause menu, and quitting the game.
- **User-Friendly Interface**: The launcher features a clean and intuitive interface, with a designated space for a logo image in the top bar.

## Project Structure

- **GameLauncher.sln**: Solution file that organizes the project and its dependencies.
- **App.xaml**: Defines application-level resources and styles.
- **MainWindow.xaml**: Main user interface layout for the game launcher.
- **Views**: Contains XAML files for different views, including settings and key bindings.
- **ViewModels**: Contains view models that manage the state and logic for the views.
- **Models**: Defines models for game settings and key bindings.
- **Resources**: Placeholder for the logo image.
- **Helpers**: Contains utility classes for command handling in the MVVM pattern.

## Setup Instructions

1. Clone the repository to your local machine.
2. Open the solution file `GameLauncher.sln` in your preferred IDE.
3. Build the solution to restore dependencies.
4. Run the application to launch the game launcher.

## Usage Guidelines

- Use the main interface to navigate to settings and key bindings.
- Adjust the settings as per your preferences and save them.
- Start the game directly from the launcher after configuring your settings.

## Contributing

Contributions are welcome! Please feel free to submit a pull request or open an issue for any enhancements or bug fixes.