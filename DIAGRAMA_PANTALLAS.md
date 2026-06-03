# Diagrama de pantallas

Este diagrama resume la navegacion principal de la aplicacion WPF ubicada en `Erp/ErpSistem`.

## Flujo general

```mermaid
flowchart TD
    A["Home.xaml<br/>Ventana principal"] --> B{"Conexion BD disponible?"}
    B -- "No" --> C["ModalMensaje<br/>Base de datos no iniciada"]
    C --> D["Cerrar sistema"]
    B -- "Si" --> E["Login.xaml"]
    E --> F{"Credenciales validas?"}
    F -- "No" --> G["ModalMensaje<br/>Credenciales incorrectas"]
    G --> E
    F -- "Si" --> H["Menu.xaml<br/>Menu por rol"]

    H --> I["VENTA<br/>Pagepedido.xaml"]
    H --> J["CAJA<br/>Pagecaja.xaml"]
    H --> K["PEDIDO<br/>Pagenotaventa.xaml"]
    H --> L["CAMBIO<br/>modal_cambio.xaml"]
    H --> M["INVENTARIO<br/>Pageproductos.xaml"]
    H --> N["Ventas del dia<br/>pageventadeldia.xaml"]
    H --> O["Caja por fecha<br/>pagecajadia.xaml"]
    H --> P["Home reportes<br/>reporteventa.xaml"]
    H --> Q["PRO.CATEGORIA<br/>ModuloCategoria.xaml"]
    H --> R["BODEGAS<br/>ModuloBodega.xaml"]
    H --> S["USUARIOS<br/>ModuloUser.xaml"]
    H --> T["FACTURAS<br/>pagefactura.xaml"]
    H --> U["SALIR / cerrar sesion"]
```

## Menus por rol

```mermaid
flowchart LR
    R1["Rol 1"] --> R1A["VENTA"]
    R1 --> R1B["CAJA"]
    R1 --> R1C["INVENTARIO"]
    R1 --> R1D["FACTURAS"]

    R2["Rol 2"] --> R2A["PEDIDO"]
    R2 --> R2B["CAMBIO"]

    R3["Rol 3"] --> R3A["Home reportes"]
    R3 --> R3B["Ventas del dia"]
    R3 --> R3C["Caja por fecha"]
    R3 --> R3D["INVENTARIO"]
    R3 --> R3E["PRO.CATEGORIA"]
    R3 --> R3F["BODEGAS"]
    R3 --> R3G["USUARIOS"]
    R3 --> R3H["FACTURAS"]

    R4["Rol 4"] --> R4A["INVENTARIO"]

    R5["Rol 5"] --> R5A["Ventas del dia"]
    R5 --> R5B["INVENTARIO"]
    R5 --> R5C["PRO.CATEGORIA"]
    R5 --> R5D["BODEGAS"]
```

## Pantallas y modales principales

```mermaid
flowchart TD
    Pedido["Pagenotaventa.xaml<br/>Pedido / nota de venta"]
    Pedido --> MCantidad["ModalCantidad.xaml"]
    Pedido --> HVenta["HabilitarProceso<br/>Autorizar Venta"]
    Pedido --> HDesc["HabilitarProceso<br/>Habilitar descuento"]
    Pedido --> MDesc["modal_descuento.xaml"]
    Pedido --> MMensaje["ModalMensaje.xaml"]

    Cambio["modal_cambio.xaml<br/>Cambio"]
    Cambio --> MCantDevol["modal_cant_devol.xaml"]
    Cambio --> HCambio["HabilitarProceso<br/>Habilitar Cambio"]
    Cambio --> MCamVen["ModalCambioVenta.xaml"]
    Cambio --> MMensaje

    Venta["Pagepedido.xaml<br/>Venta / atenciones"]
    Venta --> MDevol["ModalDevolucion.xaml"]
    Venta --> MVenta["ModalVenta.xaml"]
    Venta --> HEliminar["HabilitarProceso<br/>Eliminar atencion"]
    Venta --> BAtencion["BuscarAtencion.xaml"]
    Venta --> MMensaje

    MVenta --> MFactura["ModalFactura.xaml"]
    MVenta --> HDesc
    MVenta --> MDesc
    MVenta --> MMensaje

    Caja["Pagecaja.xaml<br/>Caja"]
    Caja --> MApertura["ModalAperturaCaja.xaml<br/>ModalAperturaCierre"]
    Caja --> MMensaje

    Inventario["Pageproductos.xaml<br/>Inventario"]
    Inventario --> MItem["ModalItem.xaml<br/>Marca / color"]
    Inventario --> MProductos["ModalProductos.xaml<br/>Transferencia productos"]
    Inventario --> MMensaje
    MProductos --> MCantidadPro["ModalCantidad.xaml<br/>ModalCantidadPro"]

    Categoria["ModuloCategoria.xaml<br/>Categorias"] --> MMensaje
    Bodega["ModuloBodega.xaml<br/>Bodegas"] --> MMensaje
    Usuarios["ModuloUser.xaml<br/>Usuarios"] --> MMensaje
    ReporteCaja["pagecajadia.xaml<br/>Caja por fecha"] --> MMensaje
    Facturas["pagefactura.xaml<br/>Facturas"] --> MMensaje
```

## Mapa rapido de archivos

| Area | Pantalla | Archivo |
| --- | --- | --- |
| Inicio | Ventana principal | `Erp/ErpSistem/MENU/Home.xaml` |
| Inicio | Login | `Erp/ErpSistem/MENU/Login.xaml` |
| Inicio | Menu | `Erp/ErpSistem/MENU/Menu.xaml` |
| Pedido | Nota de venta | `Erp/ErpSistem/PEDIDO/Pagenotaventa.xaml` |
| Pedido | Cambio | `Erp/ErpSistem/PEDIDO/modal_cambio.xaml` |
| Venta | Atenciones / venta | `Erp/ErpSistem/VENTA/Pagepedido.xaml` |
| Caja | Caja | `Erp/ErpSistem/CAJA/Pagecaja.xaml` |
| Caja | Caja por fecha | `Erp/ErpSistem/CAJA/pagecajadia.xaml` |
| Inventario | Productos | `Erp/ErpSistem/INVENTARIO/Pageproductos.xaml` |
| Inventario | Bodegas | `Erp/ErpSistem/INVENTARIO/ModuloBodega.xaml` |
| Producto | Categorias | `Erp/ErpSistem/PRODUCTO/ModuloCategoria.xaml` |
| Usuarios | Usuarios | `Erp/ErpSistem/USUARIOS/ModuloUser.xaml` |
| Reportes | Ventas del dia | `Erp/ErpSistem/REPORT_VENTA/pageventadeldia.xaml` |
| Reportes | Reporte venta | `Erp/ErpSistem/REPORT_VENTA/reporteventa.xaml` |
| Reportes | Facturas | `Erp/ErpSistem/REPORT_VENTA/pagefactura.xaml` |

## Pageproductos: pantalla, clases y entidades

Archivo principal: `Erp/ErpSistem/INVENTARIO/Pageproductos.xaml`

Code-behind: `Erp/ErpSistem/INVENTARIO/Pageproductos.xaml.cs`

Controlador: `Erp/Controller/ProductoController.cs`

### Diagrama de pantalla

```mermaid
flowchart TD
    Page["Pageproductos.xaml<br/>Modulo inventario/productos"] --> Tabs["TabControl menuCarta"]

    Tabs --> T1["PRODUCTO"]
    Tabs --> T2["INVENTARIO"]
    Tabs --> T3["MOVIMIENTOS"]
    Tabs --> T4["TRANSFERENCIA BODEGA"]

    T1 --> PForm["Formulario producto<br/>Codigo, EAN, nombre, categoria,<br/>marca, color, precio, costo,<br/>descripcion, lote, imagen"]
    T1 --> PActions["Acciones<br/>Agregar, Guardar, Cancelar,<br/>Agregar marca/color, Subir imagen"]
    T1 --> PGrid["dgrid_productos<br/>Codigo, imagen, EAN, nombre,<br/>color, marca, precio, costo,<br/>utilidad, categoria, estado, editar"]

    T2 --> IFormStart["Boton ingresar producto"]
    T2 --> IForm["Formulario inventario<br/>Categoria, producto, ubicacion,<br/>unidad medida, stock minimo, stock total"]
    T2 --> IStock["Formulario stock<br/>Ingreso/Egreso, SKU, producto,<br/>ubicacion, stock actual, cantidad"]
    T2 --> IFilter["Filtro + bodega"]
    T2 --> IGrid["gridinv<br/>Mas/Menos, editar, eliminar,<br/>cod inv, cod pro, nombre,<br/>categoria, medida, ubicacion,<br/>stock minimo, stock total"]

    T3 --> MFilters["Filtros<br/>Fecha inicio, fecha termino,<br/>producto, tipo movimiento"]
    T3 --> MGrid["gridmovimiento<br/>Id, fecha, ubicacion, SKU,<br/>producto, movimiento, cantidad,<br/>stock anterior, stock actual"]

    T4 --> TrHeader["Fecha actual"]
    T4 --> TrForm["Desde, hacia, comentario"]
    T4 --> TrActions["Cancelar, agregar producto,<br/>realizar transferencia"]
    T4 --> TrGrid["tabla_tranfer_pro<br/>Eliminar, codigo, cantidad, producto"]
```

### Flujo general de clases

```mermaid
flowchart LR
    UI["Pageproductos<br/>WPF Page"] --> Controller["ProductoController"]
    UI --> EF["ModelDataBase<br/>acceso directo EF"]

    Controller --> DB["ModelDataBase"]

    Controller --> Producto["Producto"]
    Controller --> Inventario["inventario"]
    Controller --> Categoria["Categoria"]
    Controller --> Centro["centro"]
    Controller --> Medida["medida"]
    Controller --> Operacion["operacion"]

    UI --> Transferencia["transferencia_bodega"]
    UI --> Operacion
    UI --> Modales["ModalItem<br/>ModalProductos<br/>ModalMensaje"]

    Controller --> DTOProducto["productoDTO"]
    Controller --> DTOInventario["InventarioDTO"]
    Controller --> DTOMovimiento["MovimientoDTO"]
    Controller --> DTOUnidad["UnidadMedidaDTO"]
    Controller --> DTOCategoria["CategoriaDTO"]
    UI --> DTODetalle["DetalleDTO"]
```

### Flujo de acciones principales

```mermaid
sequenceDiagram
    participant U as Usuario
    participant P as Pageproductos
    participant C as ProductoController
    participant DB as ModelDataBase
    participant API as API imagen

    U->>P: Abre pantalla
    P->>C: getCategorias(), getUnidadMedida(), getCentro(), GetCentros()
    P->>DB: carga marca, color, bodega y tipo_operacion

    U->>P: Agregar producto
    P->>C: guardarProducto(productoDTO, imagen)
    C->>DB: Inserta Producto
    C->>DB: Crea inventario inicial
    C->>API: guardarImagenAsync()

    U->>P: Editar producto
    P->>C: EditProducto(productoDTO, imagen)
    C->>DB: Actualiza Producto
    C->>API: guardarImagenAsync()

    U->>P: Buscar producto
    P->>C: getGrillaProductos(null)
    C->>DB: Consulta Producto + Categoria + marca + color

    U->>P: Agregar/editar inventario
    P->>C: GuardarInventario() / EditarInventario()
    C->>DB: Inserta o actualiza inventario

    U->>P: Ingreso/Egreso stock
    P->>DB: Crea operacion
    P->>DB: Actualiza stock_total

    U->>P: Transferencia bodega
    P->>DB: Crea transferencia_bodega
    P->>DB: Crea operaciones salida/entrada
    P->>DB: Actualiza stocks
    P->>P: Imprime ticket transferencia
```

### Clase Pageproductos

Responsabilidades principales:

- Inicializa combos, grillas y permisos visibles segun rol.
- Administra CRUD de productos desde la pestana `PRODUCTO`.
- Administra inventario, ingreso/egreso de stock y filtros desde la pestana `INVENTARIO`.
- Consulta movimientos desde la pestana `MOVIMIENTOS`.
- Realiza transferencia entre bodegas desde la pestana `TRANSFERENCIA BODEGA`.

Metodos destacados:

| Metodo | Uso |
| --- | --- |
| `Pageproductos()` | Inicializa pantalla, combos, fecha, grillas y visibilidad por rol. |
| `cargarComboCategoria()` | Carga categorias para producto e inventario. |
| `cargarComboProducto(int? codigo)` | Carga productos disponibles para inventario segun categoria. |
| `cargarCentro()` / `cargarBodega()` | Carga ubicaciones/centros. |
| `CargarUnidadMedida()` | Carga unidades de medida. |
| `cargarMarca()` / `cargarColor()` | Carga marcas y colores. |
| `cargarTipoOperacion()` | Carga tipos para filtrar movimientos. |
| `cargarGrid()` | Carga grilla de productos. |
| `FiltrarProducto()` | Filtra productos por marca, nombre, color o EAN. |
| `tb_filtro_inv_KeyUp()` | Filtra inventario por codigo, EAN o nombre. |
| `btn_addCli_Click()` | Valida y agrega un producto. |
| `btn_guardar_pro()` | Guarda cambios de un producto editado. |
| `btn_editar_Click()` | Carga producto seleccionado al formulario. |
| `btn_estado_producto_Click()` | Activa o desactiva un producto. |
| `btn_save_inv_Click()` | Guarda nuevo registro de inventario. |
| `btn_guardar_inv_Click()` | Guarda cambios de inventario. |
| `btn_editar_inv_Click()` | Carga inventario seleccionado al formulario. |
| `btn_eliminar_inv_Click()` | Elimina producto de inventario. |
| `btn_mas_Click()` / `btn_menos_Click()` | Prepara ingreso o egreso de stock. |
| `btn_save_stock_Click()` | Registra operacion y actualiza stock. |
| `actualizarStockInv(int idinventario, int cantidad)` | Actualiza stock total de inventario. |
| `btn_buscar_Click()` | Consulta movimientos por fecha/producto/tipo. |
| `AgregarProductoTransferencia_Click()` | Abre modal para seleccionar productos a transferir. |
| `RealizarTransferencia_Click()` | Valida, crea transferencia, registra salida/entrada y actualiza stocks. |
| `CrearTransferencia()` | Inserta cabecera de transferencia de bodega. |
| `PrintTicketTransferencia()` / `PrintTransferencia()` | Genera e imprime comprobante de transferencia. |
| `UploadImage_Click()` | Carga imagen local al preview. |
| `ConvertImageToBytes()` | Convierte imagen de WPF a bytes para enviar al controlador. |
| `CleanControles()` / `CleanControlesInv()` | Limpia formularios. |

### Clase ProductoController

```mermaid
classDiagram
    class ProductoController {
        +getUnidadMedida() List~UnidadMedidaDTO~
        +getFiltroProducto(texto) List~productoDTO~
        +getFiltroProductoFull(texto, idcentro) List~productoDTO~
        +getProductoXcodigo(codigo) List~productoDTO~
        +getFlete() List~productoDTO~
        +getGrillaProductos(codigo) List~productoDTO~
        +getProductoInv(categoria) List~productoInvDTO~
        +getCentro() List~CategoriaDTO~
        +getCategorias() List~CategoriaDTO~
        +geInventario() List~InventarioDTO~
        +getMovimiento() List~MovimientoDTO~
        +getMovimientoxfecha(fecha_ini, fecha_fin, producto, tipo_ope) List~MovimientoDTO~
        +guardarProducto(producto, image) Task~int~
        +guardarImagenAsync(idproducto, imagen) Task
        +RemoveProducto(codigo) int
        +RemoveProductoInv(codigo) int
        +EditProducto(pro, image) Task~int~
        +EditarInventario(inv) int
        +GuardarInventario(inv) int
        +GetCentros() List~centro~
    }
```

### Entidades y DTO principales

```mermaid
classDiagram
    class Producto {
        +int codigo
        +string nombre
        +int? categoria_codigo
        +int? precio
        +int? precio_costo
        +int? estado
        +int? idmarca
        +int? idcolor
        +string ean_codigo
        +string descripcion
        +string lote
        +string imagen1
    }

    class inventario {
        +int idinventario
        +int? idingrediente
        +int? idproducto
        +int stock_minimo
        +int stock_total
        +int idcentro
        +int idmedida
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

    class transferencia_bodega {
        +int idtrans_bodega
        +int idcentrodesde
        +int idcentrohasta
        +string descripcion
        +int login_user_revisa
        +DateTime fecharevision
        +int login_user_autoriza
        +DateTime fechaautorizacion
    }

    class Categoria {
        +int codigo
        +string nombre
        +int estado
    }

    class centro {
        +int idcentro
        +string nombre
        +int estado
        +int cansell
        +int autorizacion
    }

    class productoDTO {
        +int codigo
        +string nombre
        +int precio
        +int categoria
        +string name_categoria
        +string ean
        +string marca
        +string color
        +int? idmarca
        +int? idcolor
        +int? precio_costo
        +int? estado
        +string descripcion
        +string lote
        +string ImagenRuta
        +int utilidad
    }

    class InventarioDTO {
        +int cod_inv
        +int cod_pro
        +string nombre
        +string categoria
        +int idcategoria
        +string uni_medida
        +int iduni_medida
        +string ubicacion
        +int idubicacion
        +string stock_minimo
        +string stock_total
        +string stock_minimo_color
        +string ean
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

    Producto --> Categoria : categoria_codigo
    Producto --> inventario : idproducto
    inventario --> centro : idcentro
    inventario --> operacion : idinventario
    transferencia_bodega --> operacion : idtrans_bodega
```
