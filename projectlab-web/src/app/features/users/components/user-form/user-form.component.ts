import {
  Component,
  EventEmitter,
  Input,
  OnChanges,
  Output,
  SimpleChanges,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { UserService } from '../../services/user.service';
import { UserResponse } from '../../models/user.model';
import { ToastService } from '../../../../shared/services/toast.service';

@Component({
  selector: 'app-user-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './user-form.component.html',
  styleUrl: './user-form.component.scss',
})
export class UserFormComponent implements OnChanges {
  @Output() saved = new EventEmitter<void>();
  @Output() cancelled = new EventEmitter<void>();
  @Input() userData: UserResponse | null = null;

  form: FormGroup;
  isSubmitting = false;

  constructor(
    private fb: FormBuilder,
    private userService: UserService,
    private toast: ToastService
  ) {
    this.form = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(3)]],
      ra: ['', [Validators.required]],
      email: ['', [Validators.required, Validators.email]],
      role: ['Student', Validators.required],
      phoneNumber: ['', Validators.required],

      cep: ['', Validators.required],
      logradouro: ['', Validators.required],
      bairro: ['', Validators.required],
      cidade: ['', Validators.required],
      estado: ['', Validators.required],
    });
  }

  ngOnChanges(changes: SimpleChanges): void {
    this.isSubmitting = false;

    if (this.userData) {
      this.form.patchValue(this.userData);
      this.form.get('ra')?.disable();
    } else {
      this.form.reset({ role: 'Student' });
      this.form.get('ra')?.enable();
    }
  }

  buscarCep() {
    const cepRaw = this.form.get('cep')?.value;
    if (!cepRaw) return;

    const cep = cepRaw.replace(/\D/g, '');

    if (cep.length === 8) {
      this.userService.consultarCep(cep).subscribe({
        next: (dados) => {
          if (!(dados as any).erro) {
            this.form.patchValue({
              logradouro: dados.logradouro,
              bairro: dados.bairro,
              cidade: dados.localidade,
              estado: dados.uf,
            });
          } else {
            this.toast.error('CEP não encontrado.');
          }
        },
        error: (err) => {
          console.error('Erro ao buscar CEP', err);
          this.toast.error('Erro ao consultar CEP.');
        },
      });
    }
  }

  onSubmit() {
    if (this.form.valid) {
      this.isSubmitting = true;
      const formData = this.form.getRawValue();

      const request$: Observable<any> = this.userData
        ? this.userService.update(this.userData.id, formData)
        : this.userService.create(formData);

      request$.subscribe({
        next: () => {
          this.isSubmitting = false;

          const action = this.userData ? 'atualizado' : 'criado';
          this.toast.success(`Usuário ${action} com sucesso!`);

          this.saved.emit();
        },
        error: (err: HttpErrorResponse) => {
          console.error('Erro:', err);
          this.isSubmitting = false;

          const msg = err.error?.message || err.message || 'Erro desconhecido';
          this.toast.error(`Erro ao salvar: ${msg}`);
        },
      });
    } else {
      this.form.markAllAsTouched();
      this.toast.error('Por favor, preencha todos os campos obrigatórios.');
    }
  }

  onCancel() {
    this.cancelled.emit();
  }
}
