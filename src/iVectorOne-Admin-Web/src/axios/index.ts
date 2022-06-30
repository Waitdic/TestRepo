import axios from 'axios';

const ApiCall = axios.create({
  baseURL: 'https://api.ivectorone.com/v1',
});

export default ApiCall;
