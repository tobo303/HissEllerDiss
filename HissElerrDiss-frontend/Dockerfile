FROM node:current-alpine

WORKDIR /frontend

COPY . .

RUN npm install

EXPOSE 3000

# Change our working directory to where source files are located
WORKDIR /frontend/src
ENV HISSELLERDISS_API_URL=localhost
CMD [ "npm", "start" ]