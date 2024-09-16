# Use uma imagem base com Node.js
FROM node:20-alpine

# Instale Artillery globalmente
RUN npm install -g artillery

# Defina o diretório de trabalho
WORKDIR /app

# Copie seu arquivo de configuração para dentro do container
COPY test-config.yml /app/test-config.yml

# Comando padrão para rodar Artillery
CMD ["artillery", "run", "test-config.yml"]
