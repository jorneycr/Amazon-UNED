# Sistema de amazonUNED

Este es un sistema de compras de artículos en línea, desarrollado con **ASP.NET Core MVC** y **Entity Framework Core**. La aplicación permite a los usuarios registrarse, iniciar sesión, gestión de pedidos, y gestión de carito de compras. También incluye un sistema de autenticación basado en **ASP.NET Identity**.

## Video de Presentación del proyecto

[LINK VIDEO](#)


## Funcionalidades

1. **Registro e Inicio de Sesión de Usuarios**: Los usuarios deben poder registrarse y autenticarse en el sistema. Validación de datos de entrada y manejo de errores.
2. **Navegación y Búsqueda de Productos**: Permitir a los usuarios navegar por categorías de productos y buscar productos 
por nombre. Mostrar resultados de búsqueda relevantes y filtrables.
3. **Gestión del Carrito de Compras**: Los usuarios deben poder añadir, actualizar y eliminar productos del carrito de 
compras. Mostrar el resumen del carrito con el precio total.
4. **Procesamiento de Pedidos**: Integrar un sistema de pago simulado (puede ser una pasarela de pago ficticia). Confirmar pagos y generar un recibo o ticket digital.
5. **Gestión de Pedidos**: Los usuarios pueden ver su historial de pedidos y detalles de los mismos. Implementar funcionalidad para cancelar pedidos dentro de un tiempo determinado.

## Estructura del Proyecto

- **Modelos**:
  - **Productos**: Información sobre los productos disponibles, incluyendo nombre, 
descripción, precio, categoría y stock.
  - **Usuarios**: Información sobre los usuarios registrados, historial de compras y datos 
personales.
  - **Pedidos**: Detalles de los pedidos, incluyendo usuario, productos seleccionados, 
cantidad, precio total y estado del pedido.

- **Vistas**:
  - **Página Principal**
  - **Lista de Productos**
  - **Detalles de Producto**
  - **Carrito de Compras**
  - **Confirmación de Pedido**

- **Controladores**:
  - **Controlador de Productos**
  - **Controlador de Carrito**
  - **Controlador de Pedido**
  - **Controlador de Usuario**

## Tecnologías Utilizadas

- **ASP.NET Core MVC**: Framework principal de la aplicación.
- **Entity Framework Core**: ORM utilizado para interactuar con la base de datos SQL Server en Azure.
- **ASP.NET Identity**: Para manejar la autenticación y el registro de usuarios.
- **SQL Server**: Base de datos relacional para almacenar usuarios, productos y pedidos.

## Instalación

1. Clona el repositorio:

```js
git clone https://github.com/jorneycr/Amazon-UNED
```

2. Restaura las dependencias

```js
dotnet restore
```

3. Instalcion de SQL SERVER

```js
 Sql Server
```

3. Ajustas credenciales en el archivo de appsettings.json

```js
         "DefaultConnection":"Server=tcp:jorney.database.windows.net,1433;Initial Catalog=Amazon;Persist Security Info=False;User ID=jorney;Password=TU_PASSWORD*;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
```

4. Si no tienes un certificado personalizado, puedes generar uno de desarrollo con

```js
     dotnet dev-certs https --trust
```

5. Ejecución del Proyecto

```js
dotnet run

dotnet run --launch-profile https

```


6. Ejecución del Proyecto con watch

```js
dotnet watch run

dotnet watch run --launch-profile https
```
7. URL

```js
     https://localhost:7176/
```


## Migraciones de Base de Datos usando la CLI de .NET

1. Crea una nueva migración, si ya existen, ir al paso 2

```js
dotnet ef migrations add InitialMigration
```

2. Aplica las migraciones

```js
dotnet ef database update
```
