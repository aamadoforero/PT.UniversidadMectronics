# PRUEBA TÉCNICA - APLICACIÓN WEB

1. Los resultados deben ser enviados en un documento adjunto o subidos a un repositorio GitHub al cual se pueda acceder.
2. El desarrollo debe realizarse en **.NET**, **React** y **SQL Server**.

La prueba consiste en crear una aplicación para el registro de estudiantes que cumpla con los siguientes requisitos funcionales:

1. Realizar un CRUD que permita a un estudiante registrarse en línea para la inscripción de materias.
2. El estudiante está bajo un programa de créditos.
3. Existen 10 materias disponibles.
4. Cada materia equivale a 3 créditos.
5. El estudiante solo podrá seleccionar 3 materias.
6. Hay 5 profesores, cada uno imparte 2 materias.
7. Un estudiante no podrá tener clases con el mismo profesor en más de una materia.
8. Cada estudiante puede ver en línea los registros de otros estudiantes.
9. El estudiante podrá ver solo el nombre de los alumnos con quienes compartirá cada clase.

## Requisitos técnicos

* Aplicación de patrones de diseño.
* Creación de pruebas unitarias.
* Clean Code.
* Inyección de dependencias.
* Documentación técnica que describa el flujo de la aplicación y los endpoints de la API.

---

## Recomendaciones Adicionales

Para hacer la prueba más completa y demostrar habilidades en el desarrollo de aplicaciones robustas, se sugieren los siguientes puntos adicionales:

### 1. Autenticación y Autorización
* **Autenticación de Usuarios:** Implementar un sistema de autenticación para que solo los estudiantes registrados puedan acceder a la aplicación. Esto puede incluir roles de usuario (ej., estudiante, administrador).
* **Autorización de Acceso:** Controlar los permisos de acceso a ciertas funcionalidades. Los estudiantes deberían poder ver solo la información que les corresponde y tener permisos restringidos para modificar datos ajenos.

### 2. Validación de Datos
* **Validación en Cliente y Servidor:** Implementar validaciones tanto en el lado del cliente (JavaScript) como en el servidor para asegurar que los datos sean consistentes y evitar posibles errores.
* **Restricciones de Registro de Materias:** Validar que no se inscriban más de tres materias y que no haya duplicación de profesores en las materias seleccionadas.

### 3. Manejo de Errores y Log de Eventos
* **Manejo Centralizado de Errores:** Implementar un sistema de manejo de errores que capture y gestione errores, proporcionando mensajes claros al usuario.
* **Registro de Eventos (Logging):** Guardar eventos importantes y errores en un archivo de log o en una base de datos para facilitar la depuración.

### 4. Diseño de API RESTful
* **Endpoints RESTful:** Definir endpoints RESTful bien estructurados para el CRUD de materias y estudiantes.
* **Estandarización de Respuestas:** Usar respuestas JSON estandarizadas con códigos de estado HTTP adecuados y detalles sobre la operación realizada.
* **Paginación en Consultas:** Implementar paginación en las consultas de estudiantes para manejar grandes volúmenes de datos eficientemente.

### 5. Patrones de Diseño y Arquitectura
* **Patrón Repository:** Utilizar el patrón Repository para abstraer el acceso a datos, facilitando futuras modificaciones.
* **Patrón Service Layer:** Crear servicios para encapsular la lógica de negocio y separar la lógica del controlador.
* **Aplicación de Principios SOLID:** Diseñar la arquitectura respetando los principios SOLID, mejorando así la escalabilidad y el mantenimiento del código.

### 6. Pruebas Automatizadas
* **Pruebas Unitarias:** Crear pruebas unitarias para las funciones críticas, como el cálculo de créditos y la selección de materias.
* **Pruebas de Integración:** Probar la interacción de los endpoints con la base de datos.
* **Pruebas Funcionales:** Realizar pruebas end-to-end para simular el flujo completo del usuario.

### 7. Interfaz de Usuario y Experiencia de Usuario (UI/UX)
* **Diseño Responsivo:** Asegurar que la aplicación sea accesible desde dispositivos móviles.
* **Interactividad y Retroalimentación al Usuario:** Añadir notificaciones o mensajes para informar al usuario sobre el éxito o fallo de sus acciones.

### 8. Documentación Completa
* **Documentación de API con Swagger:** Incluir Swagger para documentar los endpoints de la API REST, facilitando su uso y prueba.
* **Manual de Usuario:** Incluir un manual de usuario para facilitar el entendimiento del uso de la aplicación.

### 9. Despliegue
* **Despliegue en la Nube:** Desplegar la aplicación en una plataforma en la nube (Azure o AWS) para un entorno de prueba en producción. (Opcional)

### 10. Optimización de Rendimiento
* **Optimización de Consultas SQL:** Usar índices en la base de datos para optimizar el rendimiento de las consultas.
* **Cacheo de Datos:** Implementar mecanismos de cacheo para mejorar la velocidad de carga.
* **Optimización de Imágenes y Carga Diferida:** Mejorar el rendimiento en la UI mediante carga diferida de imágenes o recursos.

### 11. Seguridad
* **Protección Contra Inyecciones SQL:** Utilizar consultas parametrizadas para evitar ataques de inyección SQL.
* **Protección Contra Ataques de CSRF y XSS:** Aplicar técnicas de seguridad en la UI para prevenir ataques.
* **Cifrado de Datos Sensibles:** Cifrar contraseñas e información sensible, tanto en almacenamiento como en transmisión.

### 12. Escalabilidad y Extensibilidad
* **Diseño Modular:** Asegurar que la aplicación esté diseñada para facilitar la inclusión de nuevas características (más materias, profesores, o roles de usuario).
* **Microservicios:** Considerar una arquitectura de microservicios para mejorar la escalabilidad y manejo de componentes independientes.

---

## Conclusión

Al incluir estos puntos adicionales, la prueba técnica no solo evaluará las habilidades básicas de CRUD y manipulación de datos, sino también competencias en arquitectura de software, patrones de diseño, seguridad, optimización y pruebas, aspectos clave en el desarrollo de una aplicación web robusta y escalable.