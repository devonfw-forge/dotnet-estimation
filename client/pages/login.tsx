import { LoginForm } from "../app/Components/Features/Authentication/Components/Login";
import { useAuthStore } from "../app/Components/Features/Authentication/Stores/AuthStore";

export default function Login() {
  const { user } = useAuthStore();

  return <LoginForm />;
}
