# 🗺️ SkillMap - Gestor de Recursos Educativos

SkillMap es una aplicación móvil desarrollada en **.NET MAUI** con backend en **ASP.NET Core API**, diseñada para facilitar la distribución y gestión de material educativo entre docentes y estudiantes.

## 📝 Caso de Uso (Descripción)

El sistema resuelve la necesidad de centralizar el material de estudio (PDFs, videos, enlaces) que actualmente se pierde en chats o correos.

* **👨‍🏫 Docente:** Inicia sesión, visualiza sus materias asignadas y carga recursos (con título, descripción, link y foto de portada). Puede generar reportes PDF de su material como evidencia.
* **🎓 Estudiante:** Inicia sesión, navega por semestres, selecciona sus materias y consume el contenido subido por el profesor. Puede descargar los recursos.
* **🛡️ Admin:** Se encarga de dar de alta usuarios y materias en el sistema.

---

## 📊 Diagrama Entidad-Relación (DER)

> *Este diagrama se genera automáticamente con código Mermaid.*

```mermaid
erDiagram
    %% --- ENTIDADES BASE (Requerimiento: Usuarios y Roles) ---
    ROLES {
        int ID_Rol PK
        string Rol "Admin, Docente, Estudiante"
    }

    USUARIOS {
        int ID PK
        string Nombre
        string Apellido_P
        string Apellido_M
        string Email
        string Password
        int ID_Rol FK
    }

    %% --- ENTIDADES PRINCIPALES (Requerimiento: 3 entidades extra) ---
    MATERIAS {
        int ID_Materia PK
        string Nombre
        string Semestre
        int ID_Docente FK "Usuario dueño de la materia"
    }

    RECURSOS {
        int ID_Recurso PK
        string Titulo
        string Descripcion
        string Link "Ruta local o URL (MediaPicker)"
        datetime Fecha_Carga
        int Id_Tipo_Recurso FK
    }

    TIPO_RECURSOS {
        int Id_Tipo_Recurso PK
        string Nombre "PDF, Video, Link"
    }

    %% --- RELACIONES / TABLAS INTERMEDIAS ---
    
    %% Para inscripciones de alumnos
    USUARIOS_MATERIAS {
        int Id_Usuario FK
        int Id_Materia FK
    }

    %% Para que un recurso pueda estar en varias materias (Opcional, pero tu diagrama lo tenía)
    MATERIAS_RECURSOS {
        int Id_Materia FK
        int Id_Recurso FK
    }

    %% Para comentarios/calificaciones
    RECURSOS_FEEDBACKUSUARIO {
        int Id_Recurso FK
        int Id_Usuario FK
        string Feedback
    }

    %% --- DEFINICIÓN DE RELACIONES ---
    ROLES ||--|{ USUARIOS : "define el rol de"
    
    %% Relación 1:N (Docente crea muchas materias)
    USUARIOS ||--o{ MATERIAS : "administra (Docente)"

    %% Relación 1:N (Un tipo define muchos recursos)
    TIPO_RECURSOS ||--|{ RECURSOS : "clasifica"

    %% Relaciones Muchos a Muchos (Tablas intermedias)
    USUARIOS ||--|{ USUARIOS_MATERIAS : "se inscribe"
    MATERIAS ||--|{ USUARIOS_MATERIAS : "tiene alumnos"

    MATERIAS ||--|{ MATERIAS_RECURSOS : "contiene"
    RECURSOS ||--|{ MATERIAS_RECURSOS : "es usado en"

    USUARIOS ||--|{ RECURSOS_FEEDBACKUSUARIO : "comenta"
    RECURSOS ||--|{ RECURSOS_FEEDBACKUSUARIO : "recibe feedback"
```

---

## 🔐 Matriz de Permisos por Rol

| Funcionalidad | 🛡️ Admin | 👨‍🏫 Teacher | 🎓 Student |
| :--- | :---: | :---: | :---: |
| **Login (Autenticación)** | ✅ | ✅ | ✅ |
| **Ver Materias** | ✅ | ✅ | ✅ |
| **Ver Recursos** | ✅ | ✅ | ✅ |
| **Descargar PDF (Reporte)** | ✅ | ✅ | ✅ |
| **Crear Recursos** | ✅ | ✅ | ❌ |
| **Editar/Eliminar Recursos** | ✅ | ✅ | ❌ |
| **Subir Imágenes (MediaPicker)**| ✅ | ✅ | ❌ |
| **Asignar Materias** | ✅ | ❌ | ❌ |
| **Gestión de Usuarios** | ✅ | ❌ | ❌ |

---

## 🛠️ Tecnologías

* **.NET MAUI 9.0** (Cliente Móvil)
* **ASP.NET Core Web API** (Servidor)
* **SQLite + Entity Framework Core** (Base de Datos)
* **QuestPDF** (Generación de Reportes)
* **MVVM Toolkit** (Arquitectura)
