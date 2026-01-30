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

# Production stage
FROM nginx:stable-alpine AS production-stage
COPY --from=build-stage /app/dist /usr/share/nginx/html
# Copiamos una configuración personalizada de nginx para soportar SPA routing
COPY nginx.conf /etc/nginx/conf.d/default.conf
CMD ["nginx", "-g", "daemon off;"]
