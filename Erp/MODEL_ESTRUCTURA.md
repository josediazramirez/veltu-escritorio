# Estructura de la carpeta Model

La carpeta `Model` contiene el proyecto de biblioteca C# encargado de representar las entidades de datos y el contexto de Entity Framework usado por el sistema.

## Resumen

- Proyecto: `Model.csproj`
- Tipo de salida: biblioteca (`Library`)
- Framework objetivo: `.NET Framework 4.8`
- Namespace raiz: `Model`
- ORM: Entity Framework 6
- Proveedor de base de datos: MySQL (`MySql.Data.Entity.EF6`)
- Contexto principal: `ModelDataBase`
- Cadena de conexion usada por el contexto: `ConexionSistema`

## Arbol principal

```text
Model/
|-- App.config
|-- Model.csproj
|-- ModelDataBase.cs
|-- packages.config
|-- Migrations/
|   `-- Configuration.cs
|-- Properties/
|   `-- AssemblyInfo.cs
|-- agregado.cs
|-- atencion.cs
|-- autorizacion_proceso.cs
|-- Caja.cs
|-- Categoria.cs
|-- Centro.cs
|-- Cliente.cs
|-- color.cs
|-- Computador.cs
|-- Detalle_pedido.cs
|-- devo_producto.cs
|-- estado.cs
|-- ExtraIngrediente.cs
|-- factura.cs
|-- ingrediente.cs
|-- inventario.cs
|-- marca.cs
|-- medida.cs
|-- MedioPago.cs
|-- mov_caja.cs
|-- operacion.cs
|-- pagomov.cs
|-- Pedido.cs
|-- Producto.cs
|-- pro_agre.cs
|-- rol.cs
|-- tipo_operacion.cs
|-- transferencia_bodega.cs
|-- usuario.cs
|-- usuario_proceso.cs
`-- venta_devolucion.cs
```

> Nota: las carpetas `bin/` y `obj/` existen dentro de `Model`, pero corresponden a salida de compilacion y archivos temporales generados por Visual Studio/MSBuild, por eso no se incluyen como parte de la estructura fuente principal.

## Carpetas

| Carpeta | Proposito |
| --- | --- |
| `Migrations/` | Configuracion de migraciones de Entity Framework. |
| `Properties/` | Metadatos del ensamblado del proyecto. |
| `bin/` | Archivos compilados por configuracion (`Debug`, `Release`). |
| `obj/` | Archivos intermedios generados durante la compilacion. |

## Archivos de configuracion y proyecto

| Archivo | Descripcion |
| --- | --- |
| `Model.csproj` | Define el proyecto C#, referencias, framework objetivo y archivos incluidos en compilacion. |
| `App.config` | Configuracion del proyecto, incluida la conexion usada por Entity Framework. |
| `packages.config` | Lista paquetes NuGet usados por el proyecto. |
| `ModelDataBase.cs` | Contexto principal de Entity Framework. Declara los `DbSet` de las entidades. |
| `Properties/AssemblyInfo.cs` | Informacion del ensamblado. |
| `Migrations/Configuration.cs` | Configuracion de migraciones de Entity Framework. |

## Entidades incluidas

El proyecto compila las siguientes clases de modelo:

| Entidad / archivo | Uso general esperado |
| --- | --- |
| `Caja.cs` | Caja o punto de manejo de efectivo. |
| `Categoria.cs` | Categoria de productos. |
| `Cliente.cs` | Datos de clientes. |
| `Computador.cs` | Equipo o terminal asociado al sistema. |
| `Detalle_pedido.cs` | Detalle de productos/items de un pedido. |
| `MedioPago.cs` | Medios de pago disponibles. |
| `Pedido.cs` | Cabecera o registro principal de pedidos. |
| `Producto.cs` | Productos del sistema. |
| `agregado.cs` | Agregados o adicionales configurables. |
| `pro_agre.cs` | Relacion entre producto y agregado. |
| `medida.cs` | Unidades o medidas asociadas a productos. |
| `Centro.cs` | Centro, sucursal o unidad operativa. |
| `inventario.cs` | Datos de inventario. |
| `operacion.cs` | Operaciones del sistema. |
| `estado.cs` | Estados usados por procesos o registros. |
| `rol.cs` | Roles de usuario. |
| `usuario.cs` | Usuarios del sistema. |
| `marca.cs` | Marcas de productos. |
| `color.cs` | Colores o atributos asociados a productos. |
| `mov_caja.cs` | Movimientos de caja. |
| `pagomov.cs` | Pagos asociados a movimientos. |
| `venta_devolucion.cs` | Registro de devoluciones de venta. |
| `atencion.cs` | Atenciones o procesos de servicio. |
| `devo_producto.cs` | Productos incluidos en devoluciones. |
| `transferencia_bodega.cs` | Transferencias entre bodegas. |
| `usuario_proceso.cs` | Relacion entre usuarios y procesos. |
| `autorizacion_proceso.cs` | Autorizaciones asociadas a procesos. |
| `tipo_operacion.cs` | Tipos de operacion. |
| `factura.cs` | Facturas. |
| `ingrediente.cs` | Ingredientes asociados al dominio de productos. |
| `ExtraIngrediente.cs` | Ingredientes extra o adicionales. |

## Contexto Entity Framework

`ModelDataBase` hereda de `DbContext` y usa la configuracion MySQL de Entity Framework:

```csharp
[DbConfigurationType(typeof(MySql.Data.Entity.MySqlEFConfiguration))]
public partial class ModelDataBase : DbContext
```

El constructor apunta a la cadena de conexion `ConexionSistema`:

```csharp
public ModelDataBase()
    : base("name=ConexionSistema")
{
}
```

## DbSet declarados

```text
Caja
Categoria
Cliente
Computador
Detalle_pedido
MedioPago
Pedido
Producto
agregado
pro_agre
medida
centro
inventario
operacion
estado
rol
usuario
marca
color
mov_caja
pagomov
venta_devolucion
atencion
devo_producto
transferencia_bodega
usuario_proceso
autorizacion_proceso
tipo_operacion
facturas
```

## Dependencias NuGet principales

| Paquete | Version | Uso |
| --- | --- | --- |
| `EntityFramework` | `6.0.0` | ORM principal. |
| `MySql.Data` | `6.9.12` | Conector MySQL. |
| `MySql.Data.Entity` | `6.9.9` | Proveedor MySQL para Entity Framework 6. |
| `BouncyCastle` | `1.8.5` | Criptografia requerida por dependencias. |
| `Google.Protobuf` | `3.14.0` | Serializacion/protobuf requerida por dependencias. |
| `K4os.Compression.LZ4` | `1.1.11` | Compresion LZ4. |
| `K4os.Compression.LZ4.Streams` | `1.1.11` | Streams para LZ4. |
| `K4os.Hash.xxHash` | `1.0.6` | Hashing xxHash. |
| `System.Buffers` | `4.5.1` | Buffers de memoria. |
| `System.Memory` | `4.5.3` | Tipos de memoria modernos. |
| `System.Runtime.CompilerServices.Unsafe` | `4.5.2` | APIs internas de bajo nivel requeridas por dependencias. |
