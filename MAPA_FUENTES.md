# Mapa del Fuente - Proyecto ERP

Este documento proporciona una visión general de la estructura del código fuente, la arquitectura y los módulos funcionales del sistema ERP.

## 1. Estructura de Proyectos

El sistema está organizado en cuatro proyectos principales dentro de la carpeta `Erp/`, siguiendo una arquitectura multicapa:

| Proyecto | Carpeta | Responsabilidad |
| --- | --- | --- |
| **ErpSistem** | `Erp/ErpSistem/` | Capa de Presentación (UI). Desarrollada en WPF. |
| **Controller** | `Erp/Controller/` | Capa de Lógica de Negocio. Orquestación entre UI y Datos. |
| **DTO** | `Erp/DTO/` | Objetos de Transferencia de Datos. Contratos entre capas. |
| **Model** | `Erp/Model/` | Capa de Acceso a Datos. Entidades y Contexto EF6. |

## 2. Tecnologías Utilizadas

- **Framework:** .NET Framework 4.8
- **Lenguaje:** C#
- **UI:** Windows Presentation Foundation (WPF) con MahApps.Metro
- **ORM:** Entity Framework 6 (EF6)
- **Base de Datos:** MySQL (usando `MySql.Data.Entity`)
- **Reportes:** LiveCharts para gráficos y tickets de impresión directos.

## 3. Módulos de Negocio (UI)

La aplicación WPF se organiza en las siguientes áreas funcionales:

| Área | Pantallas Principales | Descripción |
| --- | --- | --- |
| **CAJA** | `Pagecaja.xaml`, `pagecajadia.xaml` | Apertura/cierre de caja, movimientos y arqueos. |
| **INVENTARIO** | `Pageproductos.xaml`, `ModuloBodega.xaml` | Gestión de productos, stock, bodegas y transferencias. |
| **VENTA** | `Pagepedido.xaml`, `ModalVenta.xaml` | Toma de pedidos, atenciones y facturación. |
| **PEDIDO** | `Pagenotaventa.xaml`, `modal_cambio.xaml` | Notas de venta, devoluciones y cambios. |
| **PRODUCTO** | `ModuloCategoria.xaml` | Configuración de categorías de productos. |
| **REPORTES** | `reporteventa.xaml`, `pageventadeldia.xaml` | Informes de ventas y estados del sistema. |
| **USUARIOS** | `ModuloUser.xaml` | Administración de usuarios, roles y permisos. |

## 4. Arquitectura y Flujo de Datos

```mermaid
flowchart TD
    subgraph Capa_Presentacion [ErpSistem (WPF)]
        UI[Views .xaml]
        CB[Code-behind .xaml.cs]
    end

    subgraph Capa_Logica [Controller]
        Ctrl[ProductoController, ClienteController, etc.]
    end

    subgraph Capa_Datos [Model & DTO]
        Entities[Entidades EF6]
        Context[ModelDataBase Context]
        DataTransfer[DTOs]
    end

    DB[(MySQL Database)]

    UI <--> CB
    CB <--> DataTransfer
    CB <--> Ctrl
    Ctrl <--> Context
    Context <--> Entities
    Context <--> DB
```

## 5. Diagramas Detallados

Existen documentos de diagramas especializados para profundizar en cada área:

- [Diagrama de Entidades](DIAGRAMA_ENTIDADES.md): Estructura detallada de la base de datos y relaciones.
- [Diagrama de Pantallas](DIAGRAMA_PANTALLAS.md): Flujo de navegación y modales de la UI.
- [Tab PRODUCTO](DIAGRAMA_TAB_PRODUCTO.md): Detalle funcional del módulo de productos.
- [Tab INVENTARIO](DIAGRAMA_TAB_INVENTARIO.md): Detalle funcional del módulo de inventario.
- [Tab MOVIMIENTOS](DIAGRAMA_TAB_MOVIMIENTOS.md): Detalle funcional del historial de movimientos.
- [Tab TRANSFERENCIA](DIAGRAMA_TAB_TRANSFERENCIA_BODEGA.md): Detalle funcional de movimientos entre bodegas.

## 6. Localización de Archivos Clave

- **Configuración de DB:** `Erp/Model/App.config` (Cadena `ConexionSistema`)
- **Punto de Entrada UI:** `Erp/ErpSistem/MENU/Home.xaml`
- **Contexto EF:** `Erp/Model/ModelDataBase.cs`
- **Lógica Principal:** `Erp/Controller/`
