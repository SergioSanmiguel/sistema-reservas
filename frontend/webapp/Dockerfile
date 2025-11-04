# Etapa 1: construir la aplicación Angular
FROM node:20 AS build
WORKDIR /app
COPY package*.json ./
RUN npm install
COPY . .
RUN npm run build -- --configuration production

# Etapa 2: servir la app con Nginx
FROM nginx:alpine
COPY --from=build /app/dist/reservas-app /usr/share/nginx/html
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
