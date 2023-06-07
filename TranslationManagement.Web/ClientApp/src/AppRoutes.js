import { Translators } from "./components/Translators";
import { TranslationJobs } from "./components/TranslationJobs";
import { Home } from "./components/Home";

const AppRoutes = [
  {
    index: true,
    element: <Home />
  },
  {
    path: '/translators',
    element: <Translators />
  },
  {
    path: '/translation-jobs',
    element: <TranslationJobs />
  }
];

export default AppRoutes;
