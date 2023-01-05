# ==== CONFIGURE =====
# Use a Node 16 base image
FROM node:lts
# Set the working directory to /app inside the container
WORKDIR /app
# Copy app files
COPY . .
# ==== BUILD =====
# Install dependencies (npm ci makes sure the exact versions in the lockfile gets installed)
RUN npm ci
# Build the app
RUN npm run build
# ==== RUN =======
# Set the env to "production"
ENV NODE_ENV development
# Expose the port on which the app will be running
EXPOSE 3000
# Start the app
CMD [ "npm", "start"]