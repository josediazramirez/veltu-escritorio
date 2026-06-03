# Diagrama tab PRODUCTO - Pageproductos

Este documento describe solo la pestana `PRODUCTO` de `Pageproductos.xaml`.

Archivo pantalla: `Erp/ErpSistem/INVENTARIO/Pageproductos.xaml`

Code-behind: `Erp/ErpSistem/INVENTARIO/Pageproductos.xaml.cs`

Controlador: `Erp/Controller/ProductoController.cs`

## Pantalla

```mermaid
flowchart TD
    Tab["Tab PRODUCTO<br/>Pageproductos.xaml"] --> Form["Formulario producto"]
    Tab --> Grid["dgrid_productos"]
    Tab --> ImageBox["Imagen producto"]

    Form --> Codigo["codigo<br/>TextBox deshabilitado"]
    Form --> Ean["tb_ean<br/>EAN"]
    Form --> Nombre["tb_nombre<br/>Nombre"]
    Form --> Categoria["cbx_categoria<br/>Categoria"]
    Form --> Marca["cbx_marca<br/>Marca"]
    Form --> Color["cbx_color<br/>Color"]
    Form --> Descripcion["tb_descripcion<br/>Descripcion"]
    Form --> Lote["tb_lote<br/>Lote"]
    Form --> Precio["tb_precio<br/>Precio venta"]
    Form --> Costo["tb_precio_costo<br/>Precio costo"]
    Form --> Filtro["tb_filtro_producto<br/>Filtro"]

    Form --> Acciones["Acciones"]
    Acciones --> Add["btn_addCli<br/>Agregar"]
    Acciones --> Save["btn_guardar<br/>Guardar edicion"]
    Acciones --> Cancel["btn_cancelar<br/>Cancelar edicion"]
    Acciones --> AddMarca["btn_add_mar<br/>Agregar marca"]
    Acciones --> AddColor["btn_add_color<br/>Agregar color"]

    ImageBox --> Upload["UploadImage_Click<br/>Subir imagen"]
    ImageBox --> Preview["PreviewImage<br/>Vista previa"]
    ImageBox --> OpenPreview["PreviewImage_MouseLeftButtonUp<br/>Abrir imagen grande"]

    Grid --> Cols["Columnas"]
    Cols --> G1["Codigo"]
    Cols --> G2["Imagen"]
    Cols --> G3["EAN"]
    Cols --> G4["Nombre"]
    Cols --> G5["Color"]
    Cols --> G6["Marca"]
    Cols --> G7["Precio"]
    Cols --> G8["Precio costo"]
    Cols --> G9["Utilidad"]
    Cols --> G10["Categoria"]
    Cols --> G11["Estado"]
    Cols --> G12["Editar"]
```

## Flujo de uso

```mermaid
sequenceDiagram
    participant U as Usuario
    participant P as Pageproductos
    participant C as ProductoController
    participant DB as ModelDataBase
    participant API as API imagen

    U->>P: Abre Pageproductos
    P->>C: getCategorias()
    P->>DB: carga marca y color

    U->>P: Escribe filtro
    P->>C: getGrillaProductos(null)
    C->>DB: consulta Producto + Categoria + marca + color
    P->>P: FiltrarProducto()
    P-->>U: Muestra productos filtrados

    U->>P: Agregar producto
    P->>P: valida nombre, categoria y precio
    P->>P: ConvertImageToBytes(PreviewImage.Source)
    P->>C: guardarProducto(productoDTO, imagen)
    C->>DB: inserta Producto
    C->>DB: crea inventario inicial
    C->>API: guardarImagenAsync()
    P-->>U: Producto agregado / producto existente

    U->>P: Editar producto
    P->>P: btn_editar_Click()
    P-->>U: Carga datos al formulario
    U->>P: Guardar
    P->>C: EditProducto(productoDTO, imagen)
    C->>DB: actualiza Producto
    C->>API: guardarImagenAsync()
    P-->>U: Producto actualizado / sin cambios

    U->>P: Cambiar estado
    P->>DB: busca Producto por codigo
    P->>DB: actualiza estado
    P-->>U: Producto activado/desactivado
```

## Clases relacionadas

```mermaid
classDiagram
    class Pageproductos {
        -ProductoController controller
        -ModelDataBase model
        +Pageproductos()
        +cargarComboCategoria()
        +cargarGrid()
        -FiltrarProducto()
        -TextBox_TextChanged(sender, e)
        -btn_addCli_Click(sender, e)
        -btn_guardar_pro(sender, e)
        -btn_editar_Click(sender, e)
        -btn_cancelar_pro_Click(sender, e)
        -btn_estado_producto_Click(sender, e)
        -btn_add_mar_Click(sender, e)
        -btn_add_color_Click(sender, e)
        -UploadImage_Click(sender, e)
        -PreviewImage_MouseLeftButtonUp(sender, e)
        -Image_MouseDown(sender, e)
        -ConvertImageToBytes(imageSource) byte[]
        +CleanControles()
        -sintilde(text) string
        -getText(value) string
    }

    class ProductoController {
        +getCategorias() List~CategoriaDTO~
        +getGrillaProductos(codigo) List~productoDTO~
        +guardarProducto(producto, image) Task~int~
        +EditProducto(pro, image) Task~int~
        +guardarImagenAsync(idproducto, imagen) Task
        +RemoveProducto(codigo) int
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

    class Categoria {
        +int codigo
        +string nombre
        +int estado
    }

    class marca {
        +int idmarca
        +string nombre
    }

    class color {
        +int idcolor
        +string nombre
    }

    Pageproductos --> ProductoController
    Pageproductos --> ModelDataBase
    ProductoController --> Producto
    ProductoController --> productoDTO
    ProductoController --> Categoria
    Producto --> Categoria : categoria_codigo
    Producto --> marca : idmarca
    Producto --> color : idcolor
```

## Metodos de la pestana PRODUCTO

| Metodo | Funcion |
| --- | --- |
| `cargarComboCategoria()` | Carga categorias activas en `cbx_categoria`. |
| `cargarGrid()` | Carga `dgrid_productos` por categoria o todos los productos. |
| `FiltrarProducto()` | Filtra productos por marca, nombre, color o EAN. |
| `TextBox_TextChanged()` | Ejecuta filtro al presionar Enter en `tb_filtro_producto`. |
| `btn_addCli_Click()` | Valida formulario y crea un producto nuevo. |
| `btn_guardar_pro()` | Guarda cambios del producto editado. |
| `btn_editar_Click()` | Carga el producto seleccionado al formulario. |
| `btn_cancelar_pro_Click()` | Cancela la edicion y limpia controles. |
| `btn_estado_producto_Click()` | Activa o desactiva el producto seleccionado. |
| `btn_add_mar_Click()` | Abre `ModalItem` para crear una marca. |
| `btn_add_color_Click()` | Abre `ModalItem` para crear un color. |
| `UploadImage_Click()` | Selecciona imagen local y la muestra en `PreviewImage`. |
| `PreviewImage_MouseLeftButtonUp()` | Abre la imagen de preview en una ventana. |
| `Image_MouseDown()` | Abre la imagen de la grilla en una ventana. |
| `ConvertImageToBytes()` | Convierte la imagen a `byte[]` para enviarla al controlador. |
| `CleanControles()` | Limpia campos, combos, validaciones e imagen. |

## Datos que se guardan al crear/editar

```mermaid
flowchart LR
    Form["Formulario PRODUCTO"] --> DTO["productoDTO"]
    DTO --> C["ProductoController"]
    C --> Entity["Producto"]
    C --> Image["guardarImagenAsync"]

    DTO --> Nombre["nombre"]
    DTO --> Categoria["categoria"]
    DTO --> Precio["precio"]
    DTO --> Ean["ean"]
    DTO --> Marca["idmarca"]
    DTO --> Color["idcolor"]
    DTO --> Costo["precio_costo"]
    DTO --> Desc["descripcion"]
    DTO --> Lote["lote"]
    Image --> Ruta["Producto.imagen1<br/>ruta retornada por API"]
```
