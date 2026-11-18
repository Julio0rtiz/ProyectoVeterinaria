# Expediente Electrónico para una Clínica Veterinaria

## Descripción

Proyecto programado del curso Desarrollo de Software IV: un sistema para que una clínica veterinaria mantenga el expediente electrónico de las mascotas que atiende.

**VeterinariaMVC** es la aplicación web en ASP.NET Core MVC (.NET 10). Contiene los módulos de administración y de veterinaria (autenticación por cookie) y la API REST (autenticación JWT). Usa SQL Server con Entity Framework Core (Code First con migraciones).

## Integrantes
Este proyecto se encuentra trabajado por los estudiantes:
- Anthony Salguera Robles - C4J684
- Dany Javier Muñoz Solano - C5H727
- Javier Villalta Perez - C4L022
- Julio Cesar Ortiz Castro - C4I158

## Estructura del proyecto
```
ProyectoVeterinaria/
└── VeterinariaMVC/
    ├── Controllers/
    ├── Data/              AppDbContext y Migrations
    ├── Models/            Entidades, enums y ViewModels
    ├── Services/          Servicio de archivos
    ├── Views/
    └── wwwroot/
```

## Inicio e Instalación

### Requisitos
- SDK de .NET 10
- SQL Server (Express o Developer)
- Herramienta de Entity Framework: `dotnet tool install --global dotnet-ef`

### Ejecutar la aplicación web
1. Revisar la cadena de conexión `Default` en `VeterinariaMVC/appsettings.json`.
2. Ejecutar desde la carpeta `VeterinariaMVC`:
   ```
   dotnet run
   ```
   Al iniciar se aplican las migraciones y se crean automáticamente los usuarios de prueba.

La clave JWT de desarrollo está en `appsettings.Development.json`. En el servidor publicado se configura una clave distinta en `Jwt:Key`.

## Usuarios de prueba

| Perfil        | Correo                | Contraseña |
|---------------|-----------------------|------------|
| Administrador | admin@veterinaria.com | Admin123!  |
| Veterinario   | vet@veterinaria.com   | Vet123!    |
| Dueño         | dueno@veterinaria.com | Dueno123!  |

## Aplicación publicada
- URL pública: pendiente

### Cómo volver a publicar
Pendiente.

## Estilo de pantallas
Todas las pantallas usan el layout común (`Views/Shared/_Layout.cshtml`), Bootstrap 5, Bootstrap Icons y la fuente Atkinson Hyperlegible. El menú es un riel lateral verde; la sección abierta aparece como una pestaña unida a la hoja de contenido. En pantallas pequeñas el riel pasa arriba y el menú se despliega con un botón.

**Colores** (definidos en `wwwroot/css/site.css`):

| Variable | Color | Uso |
|---|---|---|
| `--quirofano` | `#24493f` | Riel, botones principales, accesos clínicos |
| `--yodo` | `#c7861c` | Pestaña activa, íconos destacados y foco del teclado; nunca para texto |
| `--hoja` | `#f6f7f4` | Fondo del contenido |
| `--tinta` | `#1d2622` | Texto |
| `--borde` | `#d8dfd9` | Bordes de tarjetas, tablas y separadores |

**Mensajes al usuario:** el layout muestra automáticamente `TempData["Exito"]` y `TempData["Error"]` (vista parcial `_Mensajes`). En el controlador basta con:
```csharp
TempData["Exito"] = "Especie agregada.";
TempData["Error"] = "No se puede eliminar la especie porque tiene razas asociadas.";
```

**Seguridad de cada controlador:**

| Dónde | Atributo |
|---|---|
| Administración | `[Authorize(Roles = "Administrador")]` |
| Mascotas y expediente | `[Authorize(Roles = "Administrador,Veterinario")]` |
| API para dueños | `[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Dueno")]` |

**Pantalla de listado:** título, botón «Agregar», barra de búsqueda y la tabla dentro de una tarjeta, con las acciones como botones de ícono a la derecha.
```html
<div class="d-flex flex-wrap justify-content-between align-items-center gap-2 mb-4">
    <h1 class="h3 mb-0">Especies</h1>
    <a asp-action="Create" class="btn btn-primary"><i class="bi bi-plus-lg"></i> Agregar especie</a>
</div>

<form asp-action="Index" method="get" class="mb-3" role="search">
    <div class="input-group">
        <input type="search" name="buscar" value="@ViewData["Buscar"]" class="form-control" placeholder="Buscar por nombre" aria-label="Buscar por nombre" />
        <button type="submit" class="btn btn-outline-primary"><i class="bi bi-search"></i> Buscar</button>
    </div>
</form>

<div class="card">
    <div class="table-responsive">
        <table class="table table-hover align-middle">
            <thead>
                <tr><th>Nombre</th><th class="text-end">Acciones</th></tr>
            </thead>
            <tbody>
                <tr>
                    <td>Perro</td>
                    <td class="text-end">
                        <a asp-action="Edit" asp-route-id="1" class="btn btn-sm btn-outline-primary" title="Editar" aria-label="Editar Perro"><i class="bi bi-pencil"></i></a>
                        <a asp-action="Delete" asp-route-id="1" class="btn btn-sm btn-outline-danger" title="Eliminar" aria-label="Eliminar Perro"><i class="bi bi-trash"></i></a>
                    </td>
                </tr>
            </tbody>
        </table>
    </div>
</div>
```

**Pantalla de formulario:** el formulario va dentro de una tarjeta, con validaciones y los botones «Guardar» y «Cancelar».
```html
<div class="row">
    <div class="col-12 col-lg-8">
        <h1 class="h3 mb-4">Agregar especie</h1>
        <div class="card">
            <div class="card-body p-4">
                <form asp-action="Create" method="post">
                    <div asp-validation-summary="ModelOnly" class="alert alert-danger"></div>
                    <div class="mb-3">
                        <label asp-for="Nombre" class="form-label"></label>
                        <input asp-for="Nombre" class="form-control" />
                        <span asp-validation-for="Nombre" class="text-danger small"></span>
                    </div>
                    <div class="d-flex gap-2">
                        <button type="submit" class="btn btn-primary"><i class="bi bi-check-lg"></i> Guardar</button>
                        <a asp-action="Index" class="btn btn-outline-secondary">Cancelar</a>
                    </div>
                </form>
            </div>
        </div>
    </div>
</div>

@section Scripts {
    <partial name="_ValidationScriptsPartial" />
}
```

**Página de aviso** (vacío, error o confirmación): usar la clase `pagina-aviso`, con un ícono, un título que diga qué pasó y un botón con la acción siguiente.

**Íconos sugeridos:** agregar `bi-plus-lg`, editar `bi-pencil`, eliminar `bi-trash`, ver `bi-eye`, buscar `bi-search`, bloquear `bi-lock` / `bi-unlock`, suspender `bi-pause-circle`, descargar `bi-download`.

**Textos:** los botones dicen exactamente lo que hacen («Guardar especie», «Suspender tratamiento») y el mensaje de éxito usa el mismo verbo («Especie guardada.»). Los errores explican qué pasó y cómo resolverlo.
