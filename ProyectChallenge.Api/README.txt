Este backend fue desarrollado para el challenge de MercadoLibre, se intento cubrir varios campos relevantes para poder cumplir con la consigna.

Indice:
-------
- Arquitectura del Proyecto
	- Descripcion de cada carpeta
	- EndPoints que estan actualmente
	- Alcance tecnico
- Mejoras sugeridas
- Fuentes



1) Arquitectura del sistema:
   ------------------------

El sistema esta pensando para ser una minimal API, por lo que se espera que no sea una arquitectura robusta. Dentro de las mejoras, 
se sugiere que si el sistema escala, se cambie a una arquitectura micro-servicios.

Descripcion de las carpetas:
----------------------------
En este sistemas nos encontramos con:
- Carpeta Application: En esta carpeta se encuentra la logica de negocio, reglas definidas, validacion, y todo aquello que hace a las 
	reglas de negocio.
- Carpeta Data: En esta carpeta tenemos la base de datos que nos pide como requerimiento el challenge, ahi el sitema lo usa como fuente de datos, 
	cabe aclarar que en realidad la capa de infra se deberia conectar con una fuente de datos ( SQL por ejemplo)
- Carpeta Domain: En esta carpeta tenemos las clases que representan al nucleo del negocio.
- Carpeta EndPoints: Ya que es una arquitectura orientada a ser una minimal API, los end points estan expuestos de esta manera.
- Carpeta Handlers: Los handlers son los intermediarios entre la capa de negocio y los end points.
- Carpeta Infraestructure: En esta carpeta tenemos todo lo relacionado con el apartado de infraestructura.
- AppSettings: Se agrega la configuracion de serilog para la parte de telemetría 
- DependencyInjection.cs: Son las dependencias de los servicios, interfaces , etc.
- Proyecto ProyectChallenge.Test: En este proyecto se agregaron unas pruebas unitarias para el sistema.

Descripcion de los EndPoints:
----------------------------
En este backend nos encontramos con los siguientes end points
- EndPoint ("GET /api/products/all"): Este end point te lista todos los productos que tiene actualmente el sistema.
- EndPoint ("GET /api/products/filter"): Este end point esta diseñado para realizar busqueda con filtros personalizados.
- EndPoint ("POST /api/products/ids"): Este end point esta diseñado para buscar por varios ids de diferentes productos
- EndPoint ("POST /api/products/compare"): Este end point se utiliza para realizar comparaciones entre 2 productos que se necesite.
- EndPoint ("POST /api/products/compare/history"): Este end point te trae el historial de busquedas realizadas para tener una trazabilidad (en memoria).

Definiciones y Alcance Tecnico:
-------------------------------
- Se agregaron varios middlewares que nos permiten tener mayor control de las solictudes, entre ellos tenemos el middleware para el manejo de 
excepciones y el middleware que ofrece http para el seguimiento de solicitudes.
Por el tema de la observabilidad, se utiliza la libreria Serilog ya que esta nos permite darle una trazabilidad a las solicutdes, en un futuro 
conectarlo con api insights para una mejor telemetria y obervabilidad.
- Para el apartado de seguridad, incorporamos el RateLimiter que nos permite controlar la cantidad de solicitudes que reciben nuestros endpoints, 
esto sirve mucho para controlar que cantidad de solicitudes puede recibir y es un parametro configurable. Ademas utilizamos el apartado de 
politicas de CORS, lo tenemos en Default pero mas adelante se sugiere agregar validaciones extras.
- Se trabaja con cache, esto resulta mucho mas facil para quitarle carga a la base de datos y hace que varias consultas con datos que se repiten
no sea necesario ir a buscarlo a la base de datos.
- Para el apartado de los endpoints se agrega documentacion en cada end point con ejemplos de posibles request en el swagger para que sea 
lo mas entendible como tienen que utilizarse estos end points. Esto es muy util cuando el backend es expuesto a que lo consuman servicios
externos.
- Para la capa de negocio se agregaron validadores con fluent validation para control de diferentes propiedades o reglas de negocio para mayor
calidad.

Fuera del alcance:
- El manejo de las credenciales y roles queda fuera del alcance del sistema para fines practicos.

2) Mejoras Sugeridas:
  ------------------
- Si se apunta a un desarrollo de una Api mas robusta, seguramente se sugiere implementar un enfoque de una arquitectura limpia, 
  con capas de negocio, de infraestructura y de dominio bien separadas, para una mayor escalabilidad ( para fines practicos se separan 
  las cosas pero dentro del mismo proyecto)
- Seguramente si el sistema crece, no es una buena practica usar el domain que este directo en las respuestas, por lo que dentro de 
las mejoras se sugiere usar los Dtos para evitar que la capa de domain no este en contacto con las solicitudes
- Generalmente si la lista es grande, se sugiere trabajar con paginadores en los gets, para no mover tanto volumen de datos.
- Para el apartado de pruebas unitarias, se sugiere buscar una cobertura de codigo mayor al 90 %, quizas utilizar alguna herramienta como sonarcloud
para mayor control y calidad de los entregables.
- Se sugiere trabajar con distintos roles, ya que determinados end points, como el historico es informacion util para un grupo determinado de 
usuarios.

3) Fuentes Utilizadas:
  -------------------
- https://learn.microsoft.com/en-us/aspnet/core/tutorials/min-web-api?view=aspnetcore-9.0&tabs=visual-studio
- https://serilog.net/
- OpenAI
- https://learn.microsoft.com/en-us/aspnet/core/performance/rate-limit?view=aspnetcore-9.0
- https://learn.microsoft.com/en-us/dotnet/core/extensions/logging?tabs=command-line