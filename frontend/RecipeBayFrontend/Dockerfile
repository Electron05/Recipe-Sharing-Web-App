# Use official Node.js 20 image (LTS version)
FROM node:20-alpine

# Set working directory
WORKDIR /app

# Copy package files
COPY package*.json ./

# Install dependencies
RUN npm install -g @angular/cli@latest
RUN npm install

# Copy source code
COPY . .

# Expose port
EXPOSE 4200

# Start Angular dev server
CMD ["ng", "serve", "--host", "0.0.0.0", "--poll", "1000"]