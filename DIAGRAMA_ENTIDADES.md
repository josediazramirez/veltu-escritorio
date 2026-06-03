# Diagrama completo de entidades

Este documento resume las entidades del proyecto `Erp/Model`, sus propiedades principales y relaciones inferidas desde claves, nombres de campos y propiedades `virtual`.

Contexto EF: `Erp/Model/ModelDataBase.cs`

## DbSet registrados

| DbSet | Entidad |
| --- | --- |
| `Caja` | `Caja` |
| `Categoria` | `Categoria` |
| `Cliente` | `Cliente` |
| `Computador` | `Computador` |
| `Detalle_pedido` | `Detalle_pedido` |
| `MedioPago` | `MedioPago` |
| `Pedido` | `Pedido` |
| `Producto` | `Producto` |
| `agregado` | `agregado` |
| `pro_agre` | `pro_agre` |
| `medida` | `medida` |
| `centro` | `centro` |
| `inventario` | `inventario` |
| `operacion` | `operacion` |
| `estado` | `estado` |
| `rol` | `rol` |
| `usuario` | `usuario` |
| `marca` | `marca` |
| `color` | `color` |
| `mov_caja` | `mov_caja` |
| `pagomov` | `pagomov` |
| `venta_devolucion` | `venta_devolucion` |
| `atencion` | `atencion` |
| `devo_producto` | `devo_producto` |
| `transferencia_bodega` | `transferencia_bodega` |
| `usuario_proceso` | `usuario_proceso` |
| `autorizacion_proceso` | `autorizacion_proceso` |
| `tipo_operacion` | `tipo_operacion` |
| `facturas` | `factura` |

Nota: tambien existen archivos de entidad `ingrediente.cs` y `ExtraIngrediente.cs`, aunque no aparecen como `DbSet` directo en `ModelDataBase.cs`.

## Mapa general

```mermaid
flowchart TD
    ProductoInv["Producto / Inventario"] --> Producto
    ProductoInv --> Categoria
    ProductoInv --> marca
    ProductoInv --> color
    ProductoInv --> inventario
    ProductoInv --> medida
    ProductoInv --> centro
    ProductoInv --> operacion
    ProductoInv --> tipo_operacion
    ProductoInv --> transferencia_bodega

    Venta["Venta / Pedido"] --> Cliente
    Venta --> Pedido
    Venta --> Detalle_pedido
    Venta --> MedioPago
    Venta --> Computador
    Venta --> factura
    Venta --> atencion
    Venta --> venta_devolucion
    Venta --> devo_producto

    CajaDom["Caja"] --> Caja
    CajaDom --> mov_caja
    CajaDom --> pagomov

    Extras["Ingredientes / Agregados"] --> ingrediente
    Extras --> agregado
    Extras --> pro_agre
    Extras --> ExtraIngrediente

    Seguridad["Usuarios / Autorizacion"] --> usuario
    Seguridad --> rol
    Seguridad --> usuario_proceso
    Seguridad --> autorizacion_proceso
    Seguridad --> estado
```

## Producto e inventario

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

    class inventario {
        +int idinventario
        +int? idingrediente
        +int? idproducto
        +int stock_minimo
        +int stock_total
        +int idcentro
        +int idmedida
    }

    class centro {
        +int idcentro
        +string nombre
        +int estado
        +int cansell
        +int autorizacion
    }

    class medida {
        +int idmedida
        +string sigla
        +string nombre
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

    class tipo_operacion {
        +int idtipo_operacion
        +string nombre
        +string color
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

    Producto --> Categoria : categoria_codigo
    Producto --> marca : idmarca
    Producto --> color : idcolor
    inventario --> Producto : idproducto
    inventario --> ingrediente : idingrediente
    inventario --> centro : idcentro
    inventario --> medida : idmedida
    operacion --> inventario : idinventario
    operacion --> Producto : idproducto
    operacion --> ingrediente : idingrediente
    operacion --> tipo_operacion : idtipo_operacion
    operacion --> transferencia_bodega : idtrans_bodega
    transferencia_bodega --> centro : idcentrodesde
    transferencia_bodega --> centro : idcentrohasta
```

## Venta, pedido y devoluciones

```mermaid
classDiagram
    class Cliente {
        +int idcliente
        +int id_telefono
        +string nombre
        +string direccion
        +int? num_direccion
        +int? telefono_opc
    }

    class Computador {
        +string idComputador
        +string nombre
    }

    class MedioPago {
        +int mp_id
        +string mp_nombre
        +string mp_desc
        +int? idpedido
        +int? total
    }

    class Pedido {
        +int codigo
        +int? numero
        +int? idcliente
        +DateTime fecha
        +int precio_total
        +string observacion
        +int? mp_id
        +string cli_nombre
        +int? descuento
        +string idComputador
        +string despacho
        +int? idestado
        +int? idflete
    }

    class Detalle_pedido {
        +int id_detalle
        +int cantidad
        +int precio
        +int codigo
        +int pedido
        +int? tamanio
        +int? idinventario
        +int? precio_desc
        +int total
    }

    class factura {
        +int idfactura
        +string rut
        +string correo
        +int numero
        +int idpedido
        +DateTime fecha
        +int estado
    }

    class atencion {
        +int idatencion
        +int idestado_atencion
        +int numero_atencion
        +int idtipoatencion
        +DateTime fecha
        +int? idpedido
        +int? idventadevolucion
        +string vendedor
    }

    class venta_devolucion {
        +int idventadevolucion
        +int total_devolucion
        +int idestadodevolucion
        +int? idpedido
        +string motivo
    }

    class devo_producto {
        +int iddevo_producto
        +int idventadevolucion
        +int idproducto
        +int cantidad
        +int precio
        +int total
        +int idinventario
    }

    Cliente "1" --> "*" Pedido : idcliente
    Computador "1" --> "*" Pedido : idComputador
    MedioPago "1" --> "*" Pedido : mp_id
    Pedido "1" --> "*" Detalle_pedido : pedido
    Detalle_pedido --> Producto : codigo
    Detalle_pedido --> inventario : idinventario
    factura --> Pedido : idpedido
    atencion --> Pedido : idpedido
    atencion --> venta_devolucion : idventadevolucion
    venta_devolucion --> Pedido : idpedido
    devo_producto --> venta_devolucion : idventadevolucion
    devo_producto --> Producto : idproducto
    devo_producto --> inventario : idinventario
```

## Caja y pagos

```mermaid
classDiagram
    class Caja {
        +int codigo
        +DateTime fecha
        +DateTime hora_inicio
        +DateTime? hora_termino
        +long? total
        +string estado
        +int? total_descuento
        +int? efectivo_hay
        +int? efectivo_esperado
        +int? efectivo_diferencia
        +int? efectivo_inicio
        +int idusuario
    }

    class mov_caja {
        +int idmovcaja
        +int idtipomov
        +int total_ent
        +int total_sal
        +int idcaja
        +string observacion
        +int? idpedido
        +int? idventadevolucion
        +DateTime? mov_fecha
    }

    class pagomov {
        +int idpagomov
        +int mp_id
        +int total
        +int vuelto
        +int descuento
        +int? idmovcaja
    }

    Caja --> usuario : idusuario
    mov_caja --> Caja : idcaja
    mov_caja --> Pedido : idpedido
    mov_caja --> venta_devolucion : idventadevolucion
    pagomov --> mov_caja : idmovcaja
    pagomov --> MedioPago : mp_id
```

## Ingredientes y agregados

```mermaid
classDiagram
    class ingrediente {
        +int idingre
        +string nombre
    }

    class agregado {
        +int idagregado
        +int idingre
        +int idcategoria
        +int precio
        +int cantidad
        +int idmedida
    }

    class pro_agre {
        +int idproducto
        +int idagregado
    }

    class ExtraIngrediente {
        +int ex_id
        +string nombre
        +int ex_precio
        +int? id_detalle
        +int? id_ingre
        +int? id_agregado
    }

    agregado --> ingrediente : idingre
    agregado --> Categoria : idcategoria
    agregado --> medida : idmedida
    pro_agre --> Producto : idproducto
    pro_agre --> agregado : idagregado
    ExtraIngrediente --> Detalle_pedido : id_detalle
    ExtraIngrediente --> ingrediente : id_ingre
    ExtraIngrediente --> agregado : id_agregado
```

## Usuarios, roles y autorizaciones

```mermaid
classDiagram
    class usuario {
        +int idusuario
        +string login_usuario
        +string nombre
        +string ape_paterno
        +string ape_materno
        +string correo
        +string password
        +int idrol
        +int habilitado
        +string key
    }

    class rol {
        +int idrol
        +string nombre
        +string descripcion
    }

    class usuario_proceso {
        +int idusuario
        +int proce_id
    }

    class autorizacion_proceso {
        +int auto_id
        +DateTime auto_fecha
        +string auto_mensaje
        +string auto_estado
        +int proce_id
        +int? idusuario
    }

    class estado {
        +int idestado
        +string nombre
        +string color
    }

    usuario --> rol : idrol
    usuario_proceso --> usuario : idusuario
    autorizacion_proceso --> usuario : idusuario
```

## Tabla completa de entidades

| Entidad | Propiedades |
| --- | --- |
| `agregado` | `idagregado int`, `idingre int`, `idcategoria int`, `precio int`, `cantidad int`, `idmedida int` |
| `atencion` | `idatencion int`, `idestado_atencion int`, `numero_atencion int`, `idtipoatencion int`, `fecha DateTime`, `idpedido int?`, `idventadevolucion int?`, `vendedor string` |
| `autorizacion_proceso` | `auto_id int`, `auto_fecha DateTime`, `auto_mensaje string`, `auto_estado string`, `proce_id int`, `idusuario int?` |
| `Caja` | `codigo int`, `fecha DateTime`, `hora_inicio DateTime`, `hora_termino DateTime?`, `total long?`, `estado string`, `total_descuento int?`, `efectivo_hay int?`, `efectivo_esperado int?`, `efectivo_diferencia int?`, `efectivo_inicio int?`, `idusuario int` |
| `Categoria` | `codigo int`, `nombre string`, `estado int` |
| `centro` | `idcentro int`, `nombre string`, `estado int`, `cansell int`, `autorizacion int` |
| `Cliente` | `idcliente int`, `id_telefono int`, `nombre string`, `direccion string`, `num_direccion int?`, `telefono_opc int?`, `Pedido ICollection<Pedido>` |
| `color` | `idcolor int`, `nombre string` |
| `Computador` | `idComputador string`, `nombre string`, `Pedido ICollection<Pedido>` |
| `Detalle_pedido` | `id_detalle int`, `cantidad int`, `precio int`, `codigo int`, `pedido int`, `tamanio int?`, `idinventario int?`, `precio_desc int?`, `total int`, `ExtraIngrediente ICollection<ExtraIngrediente>`, `Pedido1 Pedido`, `Producto Producto` |
| `devo_producto` | `iddevo_producto int`, `idventadevolucion int`, `idproducto int`, `cantidad int`, `precio int`, `total int`, `idinventario int` |
| `estado` | `idestado int`, `nombre string`, `color string` |
| `ExtraIngrediente` | `ex_id int`, `nombre string`, `ex_precio int`, `id_detalle int?`, `id_ingre int?`, `id_agregado int?`, `Detalle_pedido Detalle_pedido` |
| `factura` | `idfactura int`, `rut string`, `correo string`, `numero int`, `idpedido int`, `fecha DateTime`, `estado int` |
| `ingrediente` | `idingre int`, `nombre string` |
| `inventario` | `idinventario int`, `idingrediente int?`, `idproducto int?`, `stock_minimo int`, `stock_total int`, `idcentro int`, `idmedida int` |
| `marca` | `idmarca int`, `nombre string` |
| `medida` | `idmedida int`, `sigla string`, `nombre string` |
| `MedioPago` | `mp_id int`, `mp_nombre string`, `mp_desc string`, `idpedido int?`, `total int?`, `Pedido ICollection<Pedido>` |
| `mov_caja` | `idmovcaja int`, `idtipomov int`, `total_ent int`, `total_sal int`, `idcaja int`, `observacion string`, `idpedido int?`, `idventadevolucion int?`, `mov_fecha DateTime?` |
| `operacion` | `idoperacion int`, `idtipo_operacion int`, `idinventario int`, `fecha_hora DateTime`, `cantidad int`, `idproducto int?`, `idingrediente int?`, `stock_ant int`, `stock_act int`, `idtrans_bodega int?` |
| `pagomov` | `idpagomov int`, `mp_id int`, `total int`, `vuelto int`, `descuento int`, `idmovcaja int?` |
| `Pedido` | `codigo int`, `numero int?`, `idcliente int?`, `fecha DateTime`, `precio_total int`, `observacion string`, `mp_id int?`, `cli_nombre string`, `descuento int?`, `idComputador string`, `despacho string`, `idestado int?`, `idflete int?`, `Cliente Cliente`, `Computador Computador`, `Detalle_pedido ICollection<Detalle_pedido>`, `MedioPago MedioPago` |
| `Producto` | `codigo int`, `nombre string`, `categoria_codigo int?`, `precio int?`, `precio_costo int?`, `estado int?`, `idmarca int?`, `idcolor int?`, `ean_codigo string`, `descripcion string`, `lote string`, `imagen1 string` |
| `pro_agre` | `idproducto int`, `idagregado int` |
| `rol` | `idrol int`, `nombre string`, `descripcion string` |
| `tipo_operacion` | `idtipo_operacion int`, `nombre string`, `color string` |
| `transferencia_bodega` | `idtrans_bodega int`, `idcentrodesde int`, `idcentrohasta int`, `descripcion string`, `login_user_revisa int`, `fecharevision DateTime`, `login_user_autoriza int`, `fechaautorizacion DateTime` |
| `usuario` | `idusuario int`, `login_usuario string`, `nombre string`, `ape_paterno string`, `ape_materno string`, `correo string`, `password string`, `idrol int`, `habilitado int`, `key string` |
| `usuario_proceso` | `idusuario int`, `proce_id int` |
| `venta_devolucion` | `idventadevolucion int`, `total_devolucion int`, `idestadodevolucion int`, `idpedido int?`, `motivo string` |

## Notas de lectura

- Las relaciones son inferidas por convencion de nombres y por propiedades `virtual`; no todas estan declaradas como navegacion EF en los modelos.
- Algunas tablas usan nombres en minuscula (`centro`, `inventario`, `operacion`) y otras PascalCase (`Producto`, `Pedido`, `Caja`), siguiendo los archivos existentes.
- `ingrediente` y `ExtraIngrediente` existen como clases de modelo, pero no estan expuestas como `DbSet` directo en `ModelDataBase.cs`.
