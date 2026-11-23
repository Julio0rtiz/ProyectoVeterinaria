# Proyecto Programado: Expediente Electrónico Veterinario

## Descripción
Este es el proyecto programado para el curso IF0009 - Desarrollo de Software IV. Consiste en un sistema para que una clínica veterinaria mantenga el expediente electrónico de las mascotas que atiende, con una aplicación web en ASP.NET Core MVC y una aplicación móvil en Flutter.

## Integrantes
Este proyecto se encuentra trabajado por los estudiantes:
- Anthony Salguera Robles - C4J684
- Dany Javier Muñoz Solano - C5H727
- Javier Villalta Perez - C4L022
- Julio Cesar Ortiz Castro - C4I158

## Despliegues
Este proyecto se encuentra documentado en un [repositorio de Git](https://github.com/Julio0rtiz/ProyectoVeterinaria) y alojado en una [web de Somee](https://www.veterinaria.somee.com) desde la que es accesible la aplicación web y la API que consume la aplicación móvil.

## Inicio e Instalación

### Aplicación web
En la pagina de [Somee](https://www.veterinaria.somee.com) se encuentra la aplicacion web de forma funcional.

Para ejecutar el código fuente, desde la carpeta `VeterinariaMVC`:
```
cd VeterinariaMVC
npm install
dotnet build
dotnet watch run
```

### Aplicación móvil
En la carpeta `VeterinariaAndroid/` se encuentra el APK de la aplicacion movil.

Para ejecutar el código fuente, desde la carpeta `VeterinariaAndroid/veterinaria_app`:
```
cd VeterinariaAndroid/veterinaria_app
flutter pub get
flutter run
```

### Base de datos
La aplicacion cuenta con una base de datos de SQL Server 2022 alojada en Somee correctamente, si se desea realizar pruebas locales `Base/` se encuentra un `.bak` con los datos precargados de la web.