# Guía de Contribución

¡Gracias por tu interés en contribuir a XarxesLabs! Esta guía te ayudará a entender cómo puedes colaborar con el proyecto.

## 🤝 Tipos de Contribuciones

### Para Estudiantes
- **Reportar bugs**: Si encuentras errores en el código o documentación
- **Sugerir mejoras**: Ideas para mejorar las prácticas existentes
- **Compartir soluciones**: Enfoques alternativos interesantes
- **Corregir documentación**: Typos, clarificaciones, ejemplos adicionales

### Para Profesores
- **Nuevas prácticas**: Añadir ejercicios al curriculum
- **Actualizar contenido**: Mantener al día con nuevas tecnologías
- **Revisar código**: Code review de las contribuciones
- **Mejorar plantillas**: Optimizar las estructuras base

## 📋 Proceso de Contribución

### 1. Reportar Issues
1. Busca si ya existe un issue similar
2. Usa el template apropiado
3. Proporciona información detallada:
   - Pasos para reproducir el problema
   - Comportamiento esperado vs actual
   - Entorno (OS, Unity version, etc.)
   - Screenshots si es relevante

### 2. Proponer Cambios
1. **Fork** el repositorio
2. Crea una **branch** descriptiva:
   ```bash
   git checkout -b feature/nueva-practica-websockets
   git checkout -b fix/corregir-typo-readme
   git checkout -b docs/actualizar-instalacion
   ```
3. Realiza tus cambios
4. Haz commit con mensajes descriptivos
5. Abre un **Pull Request**

### 3. Pull Request Guidelines

#### Título del PR
- `[FEATURE]` - Nueva funcionalidad
- `[FIX]` - Corrección de bug
- `[DOCS]` - Cambios en documentación
- `[REFACTOR]` - Refactoring de código
- `[TEST]` - Añadir o mejorar tests

Ejemplo: `[FEATURE] Añadir práctica de WebSockets al Módulo 2`

#### Descripción del PR
```markdown
## Descripción
Breve descripción de los cambios realizados.

## Tipo de cambio
- [ ] Bug fix
- [ ] Nueva funcionalidad
- [ ] Cambio que rompe compatibilidad
- [ ] Actualización de documentación

## Testing
- [ ] He probado mi código localmente
- [ ] He añadido tests donde era necesario
- [ ] Todos los tests existentes pasan

## Checklist
- [ ] Mi código sigue las convenciones del proyecto
- [ ] He actualizado la documentación correspondiente
- [ ] He añadido comentarios en código complejo
```

## 📝 Estándares de Código

### C# (Unity Scripts)
- Usar **PascalCase** para clases y métodos públicos
- Usar **camelCase** para variables y métodos privados
- Añadir comentarios XML para métodos públicos:
  ```csharp
  /// <summary>
  /// Conecta al servidor especificado
  /// </summary>
  /// <param name="serverIP">IP del servidor</param>
  /// <returns>True si la conexión fue exitosa</returns>
  public bool ConnectToServer(string serverIP)
  ```

### Documentación (Markdown)
- Usar headers jerárquicos (##, ###, ####)
- Añadir emojis para mejorar legibilidad
- Incluir ejemplos de código donde sea útil
- Mantener líneas menores a 100 caracteres cuando sea posible

### Estructura de Archivos
- Seguir la estructura establecida en las plantillas
- Nombrar archivos de manera descriptiva
- Usar kebab-case para nombres de carpetas

## 🧪 Testing

### Para Nuevas Prácticas
1. **Funcionalidad básica**: El código funciona como se describe
2. **Multiplataforma**: Testear en Windows/Mac/Linux si es posible
3. **Diferentes versiones Unity**: Al menos Unity 2022.3 LTS
4. **Documentación**: Verificar que las instrucciones son claras

### Para Cambios de Código
1. **Unit tests**: Añadir si es apropiado
2. **Integration tests**: Probar con otros módulos
3. **Performance**: Verificar que no se degrada el rendimiento
4. **Backward compatibility**: No romper prácticas existentes

## 📚 Recursos para Contribuidores

### Documentación Técnica
- [Unity Scripting API](https://docs.unity3d.com/ScriptReference/)
- [Unity Netcode Documentation](https://docs-multiplayer.unity3d.com/)
- [C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/inside-a-program/coding-conventions)

### Herramientas Útiles
- **Unity Editor**: Para crear y probar prácticas
- **Visual Studio Code**: Para editar documentación
- **Git**: Para control de versiones
- **Markdown editors**: Para previsualizar documentación

## 🎯 Prioridades Actuales

### Alta Prioridad
- Completar prácticas faltantes en módulos existentes
- Añadir ejemplos de código completos
- Mejorar la documentación de instalación
- Crear videos tutoriales básicos

### Media Prioridad
- Optimizar plantillas de proyecto
- Añadir más herramientas de testing
- Integrar CI/CD para validación automática
- Crear más recursos de aprendizaje

### Baja Prioridad
- Soporte para versiones anteriores de Unity
- Traducciones a otros idiomas
- Integraciones con servicios externos
- Características experimentales

## 📞 Contacto

### Para Dudas sobre Contribuciones
- **Issues**: Para reportar problemas o sugerir mejoras
- **Discussions**: Para debates más amplios sobre el proyecto
- **Email**: Para comunicación directa con los mantenedores

### Mantenedores del Proyecto
- **Profesores de la asignatura**: Revisión de contenido académico
- **Ayudantes de cátedra**: Soporte técnico y revisión de código
- **Colaboradores activos**: Estudiantes con experiencia previa

## 🙏 Reconocimiento

Todas las contribuciones son valoradas y reconocidas:
- Créditos en la documentación
- Mención en las release notes
- Posible invitación como colaborador regular
- Referencias para oportunidades académicas/profesionales

---

*¡Esperamos tus contribuciones para hacer este proyecto aún mejor!*