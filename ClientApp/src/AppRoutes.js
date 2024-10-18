import { Counter } from "./views/Counter";
import { FetchData } from "./views/FetchData";
import { Home } from "./views/Home";
import  Books  from "./views/Books"

const AppRoutes = [
  {
    index: true,
    element: <Home />
  },
  {
    path: '/counter',
    element: <Counter />
  },
  {
    path: '/fetch-data',
    element: <FetchData />
  },
  {
    path: '/bookstore',
    element: <Books/>
  }
];

export default AppRoutes;
