# Build stage
FROM node:20-alpine AS build-stage
WORKDIR /app
COPY package*.json ./
RUN npm install
COPY . .

# Argumentos para variables de entorno en tiempo de compilación
ARG VITE_API_URL
ARG VITE_MERCADOPAGO_PUBLIC_KEY

ENV VITE_API_URL=$VITE_API_URL
ENV VITE_MERCADOPAGO_PUBLIC_KEY=$VITE_MERCADOPAGO_PUBLIC_KEY

RUN npm run build

# Runtime stage
FROM nginx:alpine
COPY --from=build-stage /app/dist /usr/share/nginx/html
# Copiamos la configuración de Nginx para manejar las rutas de la SPA (Vue)
COPY nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]