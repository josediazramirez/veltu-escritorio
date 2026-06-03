# Diagrama tab MOVIMIENTOS - Pageproductos

Este documento describe solo la pestana `MOVIMIENTOS` de `Pageproductos.xaml`.

Archivo pantalla: `Erp/ErpSistem/INVENTARIO/Pageproductos.xaml`

Code-behind: `Erp/ErpSistem/INVENTARIO/Pageproductos.xaml.cs`

Controlador: `Erp/Controller/ProductoController.cs`

## Pantalla

```mermaid
flowchart TD
    Tab["Tab MOVIMIENTOS<br/>Pageproductos.xaml"] --> Filtros["Filtros"]
    Tab --> Grid["gridmovimiento"]

    Filtros --> FechaIni["fecha_ini<br/>Fecha inicio"]
    Filtros --> FechaFin["fecha_fin<br/>Fecha termino"]
    Filtros --> Producto["tbx_producto<br/>Producto"]
    Filtros --> Tipo["cbx_tipo_mov<br/>Tipo movimiento"]
    Filtros --> Buscar["btn_buscar<br/>BUSCAR"]

    Grid --> Cols["Columnas"]
    Cols --> C1["Id"]
    Cols --> C2["Fecha y hora"]
    Cols --> C3["Ubicacion"]
    Cols --> C4["SKU"]
    Cols --> C5["Producto"]
    Cols --> C6["Movimiento"]
    Cols --> C7["Cant.Mov"]
    Cols --> C8["Stock anterior"]
    Cols --> C9["Stock actual"]
```

## Flujo de uso

```mermaid
sequenceDiagram
    participant U as Usuario
    participant P as Pageproductos
    participant C as ProductoController
    participant DB as ModelDataBase

    U->>P: Abre Pageproductos
    P->>DB: cargarTipoOperacion()
    DB-->>P: tipo_operacion
    P-->>U: Carga cbx_tipo_mov

    U->>P: Define fechas, producto y tipo
    U->>P: Presiona BUSCAR
    P->>C: getMovimientoxfecha(fecha_ini, fecha_fin, producto, tipo_ope)
    C->>DB: sp_inventario_mov_x_fecha
    DB-->>C: List<MovimientoDTO>
    C-->>P: movimientos
    P-->>U: Actualiza gridmovimiento
```

## Clases relacionadas

```mermaid
classDiagram
    class Pageproductos {
        -ProductoController controller
        -ModelDataBase model
        +cargarTipoOperacion()
        -getMovimiento()
        -btn_buscar_Click(sender, e)
    }

    class ProductoController {
        +getMovimiento() List~MovimientoDTO~
        +getMovimientoxfecha(fecha_ini, fecha_fin, producto, tipo_ope) List~MovimientoDTO~
    }

    class MovimientoDTO {
        +int id
        +DateTime fecha_hora
        +int cod_pro
        +string nombre_pro
        +string movimiento
        +string cantidad
        +string ubicacion
        +string stock_ant
        +string stock_act
        +string mov_color
        +string destino
    }

    class tipo_operacion {
        +int idtipo_operacion
        +string nombre
        +string color
    }

    class operacion {
        +int idoperacion
        +int idtipo_operacion
        +int idinventario
        +DateTime fecha_hora
        +int cantidad
        +int? idproducto
        +int? idingrediente
        +int stock_ant
        +int stock_act
        +int? idtrans_bodega
    }

    Pageproductos --> ProductoController
    Pageproductos --> tipo_operacion
    ProductoController --> MovimientoDTO
    MovimientoDTO --> operacion
```

## Metodos de la pestana MOVIMIENTOS

| Metodo | Funcion |
| --- | --- |
| `cargarTipoOperacion()` | Carga los tipos de operacion en `cbx_tipo_mov`. |
| `getMovimiento()` | Carga todos los movimientos con `controller.getMovimiento()`. |
| `btn_buscar_Click()` | Consulta movimientos por fecha, producto y tipo de movimiento. |

## Datos consultados

```mermaid
flowchart LR
    Filtros["Filtros MOVIMIENTOS"] --> Controller["ProductoController"]
    Controller --> SP["sp_inventario_mov_x_fecha"]
    SP --> DTO["MovimientoDTO"]
    DTO --> Grid["gridmovimiento"]

    Filtros --> Inicio["fecha_ini"]
    Filtros --> Fin["fecha_fin"]
    Filtros --> Producto["tbx_producto"]
    Filtros --> Tipo["cbx_tipo_mov"]
```

## Observaciones

- La grilla es solo de consulta; esta pestana no crea ni modifica movimientos.
- Los movimientos se originan principalmente desde `INVENTARIO` y `TRANSFERENCIA BODEGA`.
- El color de la columna `Movimiento` viene desde `mov_color`.
