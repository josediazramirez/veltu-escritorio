# Plan de Migración: De Código Monolítico a API REST

Este documento detalla la estrategia paso a paso para migrar la lógica de acceso a datos y las reglas de negocio del sistema ERP desde la aplicación WPF actual hacia un conjunto de servicios API modernos.

## 1. Fase 1: Preparación de la Infraestructura

- **Tecnología Core:** ASP.NET Core 8.0 (Web API).
- **ORM:** Entity Framework Core (migrando desde EF6).
- **Seguridad:** Implementación de JWT (JSON Web Tokens) para autenticación y autorización basada en roles.
- **Documentación:** Swagger/OpenAPI para pruebas y contratos de interfaz.

## 2. Fase 2: Abstracción de Datos (Data Layer)

1. **Migración de Entidades:** Adaptar las clases del proyecto `Erp/Model` a convenciones de EF Core (DataAnnotations, Fluent API).
2. **Repositorio Único:** Crear una capa de persistencia que maneje el contexto de base de datos MySQL, reemplazando el uso directo de `ModelDataBase` en la UI.
3. **Mapeo de DTOs:** Utilizar AutoMapper o similares para transformar entidades de base de datos a los contratos definidos en el proyecto `Erp/DTO`.

## 3. Fase 3: Migración de Lógica de Negocio (Service Layer)

Se priorizará la migración por módulos según la documentación de microservicios existente:

### A. Módulo Producto e Inventario
- Migrar métodos de `ProductoController` a `ProductoService` e `InventarioService`.
- Exponer endpoints RESTful (POST, GET, PUT, PATCH, DELETE) definidos en `MICROSERVICIO_TAB_PRODUCTO.md`.
- Implementar la lógica de guardado de imágenes mediante el endpoint de la API.

### B. Módulo de Movimientos y Transferencias
- Centralizar la lógica transaccional de transferencias (Salida Origen + Entrada Destino) en un servicio atómico para evitar inconsistencias de stock.
- Migrar los Stored Procedures actuales a consultas LINQ optimizadas o SPs ejecutados mediante EF Core.

## 4. Fase 4: Refactorización del Cliente (WPF)

1. **Capa de Servicio API:** Crear una clase `ErpApiClient` en el proyecto WPF que utilice `HttpClient`.
2. **Inyección de Dependencias:** Configurar la aplicación WPF para inyectar los clientes de servicio en lugar de instanciar controladores locales.
3. **Migración Progresiva:** Reemplazar llamadas a `ProductoController` por llamadas asíncronas a la API, manejando estados de carga y errores HTTP.

## 5. Fase 5: Pruebas y Validación

- **Pruebas Unitarias:** Validar reglas críticas (conversión de medidas, validación de stock negativo).
- **Pruebas de Integración:** Verificar la conexión API -> MySQL.
- **Validación UI:** Asegurar que las grillas y formularios de `Pageproductos.xaml` funcionen correctamente con los datos provenientes de la API.

## 6. Cronograma Sugerido

| Semana | Actividad |
| --- | --- |
| 1 | Configuración de proyecto base API y Seguridad JWT. |
| 2 | Migración de Entidades y Módulo Producto. |
| 3 | Migración de Módulo Inventario y Movimientos. |
| 4 | Implementación de Transferencias (Transaccionalidad). |
| 5 | Refactorización de UI WPF y Pruebas finales. |
