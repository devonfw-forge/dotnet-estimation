import "../styles/globals.css";
import type { AppProps } from "next/app";
import { useRouter } from "next/router";
import { useAuthStore } from "../app/Components/Features/Authentication/Stores/AuthStore";

export default function App({ Component, pageProps }: AppProps) {
  return <Component {...pageProps} />;
}
