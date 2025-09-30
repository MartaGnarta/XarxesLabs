# Guía de Instalación y Configuración

## 🛠️ Requisitos del Sistema

### Software Necesario

1. **Unity Hub y Unity Editor**
   - Unity Hub: [Descargar desde unity.com](https://unity.com/download)
   - Unity Editor: Versión 2022.3 LTS (recomendada)
   
2. **IDE/Editor de Código**
   - Visual Studio 2022 (Windows) - Recomendado
   - Visual Studio Code (Multiplataforma) - Alternativa
   - JetBrains Rider (Multiplataforma) - Avanzado

3. **Control de Versiones**
   - Git: [Descargar desde git-scm.com](https://git-scm.com/)
   - Cliente Git (opcional): GitHub Desktop, SourceTree, etc.

### Requisitos del Sistema

- **Sistema Operativo**: Windows 10/11, macOS 10.15+, Ubuntu 18.04+
- **RAM**: Mínimo 8GB, recomendado 16GB
- **Espacio en Disco**: Al menos 10GB libres
- **Conexión a Internet**: Para descargas y prácticas de red

## 📥 Instalación Paso a Paso

### 1. Instalar Unity

1. Descarga e instala Unity Hub
2. Abre Unity Hub y ve a "Installs"
3. Haz clic en "Install Editor"
4. Selecciona la versión 2022.3 LTS
5. En los módulos, asegúrate de incluir:
   - Microsoft Visual Studio Community (Windows)
   - WebGL Build Support
   - Development Tools

### 2. Configurar el IDE

#### Visual Studio (Windows)
1. Durante la instalación de Unity, se instalará automáticamente
2. Asegúrate de tener las cargas de trabajo:
   - Desarrollo de juegos con Unity
   - Desarrollo de .NET Desktop

#### Visual Studio Code (Multiplataforma)
1. Descarga desde [code.visualstudio.com](https://code.visualstudio.com/)
2. Instala las extensiones:
   - C# (Microsoft)
   - Unity Code Snippets
   - Unity Tools

### 3. Configurar Git

```bash
# Configurar nombre y email
git config --global user.name "Tu Nombre"
git config --global user.email "tu.email@estudiants.citm.upc.edu"

# Configurar editor (opcional)
git config --global core.editor "code --wait"
```

### 4. Clonar el Repositorio

```bash
# Clonar el repositorio
git clone https://github.com/MartaGnarta/XarxesLabs.git

# Entrar al directorio
cd XarxesLabs
```

## ✅ Verificación de la Instalación

### Test Unity
1. Abre Unity Hub
2. Haz clic en "New Project"
3. Selecciona "3D" template
4. Crea el proyecto y verifica que se abra correctamente

### Test Git
```bash
git --version
# Debería mostrar la versión instalada
```

### Test IDE
1. Abre tu IDE preferido
2. Abre el proyecto Unity creado en el test anterior
3. Verifica que puedas editar scripts C#

## 🔧 Configuración Adicional

### Unity Preferences
1. Ve a Edit > Preferences > External Tools
2. Configura el External Script Editor a tu IDE preferido

### Git para Unity
1. Ve a Edit > Project Settings > Version Control
2. Selecciona "Visible Meta Files"
3. En Edit > Project Settings > Asset Serialization
4. Selecciona "Force Text"

## ❗ Problemas Comunes

### Unity no encuentra Visual Studio
- Reinstala Visual Studio con las cargas de trabajo correctas
- Verifica la configuración en Unity Preferences > External Tools

### Errores de Git con archivos grandes
- Unity genera archivos grandes que no deben versionarse
- Asegúrate de tener un .gitignore apropiado

### Problemas de red en Unity
- Verifica que el firewall permita Unity
- Algunos laboratorios pueden requerir puertos específicos

## 📞 Soporte

Si encuentras problemas durante la instalación:
1. Consulta la documentación oficial de cada herramienta
2. Revisa los issues del repositorio
3. Contacta con los profesores de la asignatura