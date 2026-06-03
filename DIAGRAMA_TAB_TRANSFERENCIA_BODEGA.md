# Diagrama tab TRANSFERENCIA BODEGA - Pageproductos

Este documento describe solo la pestana `TRANSFERENCIA BODEGA` de `Pageproductos.xaml`.

Archivo pantalla: `Erp/ErpSistem/INVENTARIO/Pageproductos.xaml`

Code-behind: `Erp/ErpSistem/INVENTARIO/Pageproductos.xaml.cs`

Modal relacionado: `Erp/ErpSistem/INVENTARIO/ModalProductos.xaml`

## Pantalla

```mermaid
flowchart TD
    Tab["Tab TRANSFERENCIA BODEGA<br/>Pageproductos.xaml"] --> Header["gr_header<br/>Titulo y fecha"]
    Tab --> Body["gr_body<br/>Datos transferencia"]
    Tab --> Grid["tabla_tranfer_pro"]

    Header --> Title["TRANSFERENCIA DE BODEGA"]
    Header --> Fecha["fecha_actual"]

    Body --> Desde["cbx_desde<br/>Centro origen"]
    Body --> Hasta["cbx_hasta<br/>Centro destino"]
    Body --> Comentario["tbx_comentario<br/>Comentario"]
    Body --> Cancelar["CancelarTransferencia"]
    Body --> Agregar["AgregarProductoTransferencia"]
    Body --> Realizar["RealizarTransferencia"]

    Agregar --> Modal["ModalProductos"]
    Modal --> Filtro["txt_filtro_pro<br/>Filtro producto"]
    Modal --> TablaModal["tabla_pro<br/>Productos origen"]
    TablaModal --> BtnAgregar["btn_agregarPro"]

    Grid --> Eliminar["btn_pro_tran_eliminar"]
    Grid --> C1["Codigo"]
    Grid --> C2["Cant.Mov"]
    Grid --> C3["Producto"]
```

## Flujo de uso

```mermaid
sequenceDiagram
    participant U as Usuario
    participant P as Pageproductos
    participant M as ModalProductos
    participant C as ProductoController
    participant DB as ModelDataBase
    participant PR as Impresora

    U->>P: Abre Pageproductos
    P->>C: GetCentros()
    P-->>U: Carga cbx_desde y cbx_hasta

    U->>P: Selecciona desde/hacia
    U->>P: Agregar producto
    P->>P: validarDesdeHasta()
    P->>M: Abre ModalProductos(origen, destino)
    U->>M: Busca producto
    M->>C: getFiltroProductoFull(texto, idcentroOrigen)
    C->>DB: sp_producto_filtro_full
    M-->>U: Lista productos con stock
    U->>M: Agrega producto y cantidad
    M->>DB: valida existencia en centro destino
    M-->>P: pro_retorna
    P-->>U: Agrega productos a tabla_tranfer_pro

    U->>P: Realizar transferencia
    P->>P: valida origen, destino, productos y comentario
    P->>DB: CrearTransferencia()
    loop Por cada producto
        P->>DB: operacion tipo 9 salida origen
        P->>DB: actualizarStockInv(origen)
    end
    loop Por cada producto
        P->>DB: operacion tipo 10 entrada destino
        P->>DB: actualizarStockInv(destino)
    end
    P->>P: PrintTicketTransferencia()
    P->>PR: PrintTransferencia(ticket)
    P-->>U: Transferencia realizada
```

## Clases relacionadas

```mermaid
classDiagram
    class Pageproductos {
        -ObservableCollection~DetalleDTO~ produc_transfer
        -AgregarProductoTransferencia_Click(sender, e)
        +validarDesdeHasta() string
        -btn_pro_tran_eliminar_Click(sender, e)
        -RealizarTransferencia_Click(sender, e)
        +limpiarTransferencia()
        +PrintTransferencia(ticket)
        +CrearTransferencia() int
        +PrintTicketTransferencia(idtransferencia, comentario, centroNombre) string
        -CancelarTransferencia_Click(sender, e)
        -actualizarStockInv(idinventario, cantidad) bool
    }

    class ModalProductos {
        +int idcentro
        +int idcentroDes
        +List~DetalleDTO~ pro_retorna
        -txt_filtro_pro_KeyUp(sender, e)
        -btn_agregarPro_Click(sender, e)
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

    class DetalleDTO {
        +int codigo
        +string producto
        +decimal? stock
        +int? idinventario
        +int total
        +int idmedida
        +string centro
    }

    class operacion {
        +int idoperacion
        +int idtipo_operacion
        +int idinventario
        +DateTime fecha_hora
        +int cantidad
        +int? idproducto
        +int stock_ant
        +int stock_act
        +int? idtrans_bodega
    }

    Pageproductos --> ModalProductos
    Pageproductos --> transferencia_bodega
    Pageproductos --> operacion
    Pageproductos --> DetalleDTO
    ModalProductos --> DetalleDTO
```

## Metodos de la pestana TRANSFERENCIA BODEGA

| Metodo | Funcion |
| --- | --- |
| `AgregarProductoTransferencia_Click()` | Valida origen/destino y abre `ModalProductos`. |
| `validarDesdeHasta()` | Valida centro origen, centro destino y que sean distintos. |
| `btn_pro_tran_eliminar_Click()` | Elimina un producto de `produc_transfer`. |
| `RealizarTransferencia_Click()` | Crea transferencia, registra operaciones, actualiza stock e imprime ticket. |
| `limpiarTransferencia()` | Limpia productos, comentario y habilita combos. |
| `PrintTransferencia()` | Imprime el ticket generado. |
| `CrearTransferencia()` | Inserta `transferencia_bodega` y retorna su id. |
| `PrintTicketTransferencia()` | Construye el texto del ticket. |
| `CancelarTransferencia_Click()` | Cancela y limpia la transferencia. |
| `ModalProductos.txt_filtro_pro_KeyUp()` | Busca productos del centro origen. |
| `ModalProductos.btn_agregarPro_Click()` | Valida cantidad, destino y agrega producto a la transferencia. |

## Datos que se guardan

```mermaid
flowchart LR
    Form["Formulario transferencia"] --> Transfer["transferencia_bodega"]
    Productos["produc_transfer"] --> Salida["operacion tipo 9<br/>Salida origen"]
    Productos --> Entrada["operacion tipo 10<br/>Entrada destino"]
    Salida --> StockOrigen["Actualiza stock origen"]
    Entrada --> StockDestino["Actualiza stock destino"]

    Transfer --> Desde["idcentrodesde"]
    Transfer --> Hasta["idcentrohasta"]
    Transfer --> Desc["descripcion"]
    Transfer --> User["login_user_revisa"]
    Transfer --> Fecha["fecharevision"]
```

## Observaciones

- Al agregar el primer producto, `cbx_desde` y `cbx_hasta` se deshabilitan para mantener consistente la transferencia.
- `ModalProductos` valida que el producto tenga stock suficiente en origen y que exista inventario en el centro destino.
- La transferencia genera dos movimientos por producto: tipo `9` para salida y tipo `10` para entrada.
- La operacion completa no esta envuelta en una transaccion; conviene corregirlo al migrar a microservicio.
